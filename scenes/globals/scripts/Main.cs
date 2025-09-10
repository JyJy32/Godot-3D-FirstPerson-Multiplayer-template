using Godot;
using System;

public partial class Main : Node
{
    [Export]
    public PackedScene DemoScene; // this scene is for debug purposes
    PlayerInfo playerInfo;
    Lobby lobby;
    MainMenu mainMenu;

    SceneSpawner SceneSpawner;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        lobby = GetNode<Lobby>("/root/Lobby");
        lobby.PlayerConnected += onPlayerConnected;
        lobby.LobbyGameStarted += StartGame;
        lobby.ReturnToMenu += onReturnToMenu;

        SceneSpawner = GetNode<SceneSpawner>("SceneSpawner");

        mainMenu = GetNode<MainMenu>("MainMenu");
        mainMenu.playerCreated += onPlayerCreated;
        mainMenu.startServer += onStartServer;
        mainMenu.joinGame += onJoinGame;

        playerInfo = new PlayerInfo(0, "", false);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    [Rpc]
    public void StartGame()
    {
        if (Multiplayer.IsServer())
        {
            Node demo = DemoScene.Instantiate();
            SceneSpawner.Spawn(demo);
            return;

        }
    }

    private void onPlayerCreated(string name)
    {
        playerInfo.peerInfo = name;
        lobby.playerInfo = playerInfo;
        lobby.updateInfo();
    }

    private void onPlayerConnected(PlayerInfo player)
    {
        lobby.PlayerLoaded();
    }

    private void onStartServer()
    {
        lobby.CreateServer();
    }

    private void onJoinGame(string address)
    {
        lobby.JoinGame();
    }

    private void onReturnToMenu()
    {
        Node level = GetNode("Level");
        foreach (Node child in level.GetChildren())
        {
            level.RemoveChild(child);
        }
        mainMenu.Visible = true;
    }
}
