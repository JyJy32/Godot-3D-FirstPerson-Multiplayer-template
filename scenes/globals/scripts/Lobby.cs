using Godot;
using System;
using System.Collections.Generic;

public partial class Lobby : Node
{
    [Signal]
    public delegate void PlayerConnectedEventHandler(PlayerInfo player);
    [Signal]
    public delegate void PlayerDisconnectedEventHandler(int peerID);
    [Signal]
    public delegate void ServerDisconnectedEventHandler();
    [Signal]
    public delegate void LobbyGameStartedEventHandler();
    [Signal]
    public delegate void ReturnToMenuEventHandler();

    const int PORT = 6969;
    const string DEFAULT_SERVER_IP = "127.0.0.1";
    const int MAX_CONNECTIONS = 4;

    bool gameStarted = false;
    //UI
    VBoxContainer lobbyInfo;
    Control menu;
    Control chat;

    public PlayerInfo playerInfo;
    public Dictionary<long, PlayerInfo> Players = new Dictionary<long, PlayerInfo>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Multiplayer.PeerConnected += onPlayerConnected;
        Multiplayer.PeerDisconnected += onPlayerDisconnected;
        Multiplayer.ConnectedToServer += onConnectedOk;
        Multiplayer.ConnectedToServer += updateInfo;
        Multiplayer.ConnectionFailed += onConnectedFail;
        Multiplayer.ServerDisconnected += onServerDisconnected;


        lobbyInfo = GetNode<VBoxContainer>("LobbyInfo");
        menu = GetNode<Control>("Menu");
        chat = GetNode<Control>("Chat");

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Multiplayer.HasMultiplayerPeer())
        {
            updateInfo();
        }
    }

    public void updateInfo()
    {
        if (Multiplayer.IsServer())
        {
            lobbyInfo.GetNode<Label>("Players").Text = "Players: " + Players.Count + "/" + MAX_CONNECTIONS;
        }
        else
        {
            lobbyInfo.GetNode<Label>("Players").Text = "Players: " + Players.Count;
        }
        VBoxContainer playerList = lobbyInfo.GetNode<VBoxContainer>("PlayerList");
        if (playerList == null)
        {
            GD.Print("PlayerList not found");
            return;
        }
        foreach (Node child in playerList.GetChildren())
        {
            playerList.RemoveChild(child);
        }
        foreach (KeyValuePair<long, PlayerInfo> entry in Players)
        {
            if (entry.Value != null)
            {
                Label playerLabel = new Label();
                playerLabel.Text = entry.Value.peerInfo + ": " + entry.Value.peerID;
                playerList.AddChild(playerLabel);
            }
        }
    }

    public void JoinGame(string address = "")
    {
        if (address == "")
        {
            address = DEFAULT_SERVER_IP;
        }
        ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
        Error error = peer.CreateClient(address, PORT);
        if (error != Error.Ok)
        {
            GD.Print("JoinGame() - Error: " + error);
            return;
        }
        Multiplayer.MultiplayerPeer = peer;
        lobbyInfo.Visible = true;
        lobbyInfo.GetNode<Button>("LeaveButton").Text = "Leave";
        chat.Visible = true;
    }

    public void CreateServer()
    {
        ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
        Error error = peer.CreateServer(PORT, MAX_CONNECTIONS);
        if (error != Error.Ok)
        {
            GD.Print("CreateGame() - Error: " + error);
            return;
        }
        Multiplayer.MultiplayerPeer = peer;
        playerInfo.isServer = true;
        playerInfo.peerID = Multiplayer.GetUniqueId();

        Players[playerInfo.peerID] = playerInfo;
        EmitSignal("PlayerConnected", playerInfo);
        lobbyInfo.Visible = true;
        lobbyInfo.GetNode<Button>("LeaveButton").Text = "Close Lobby";
        menu.Visible = true;
        chat.Visible = true;
    }

    public void removeMPPeer()
    {
        Multiplayer.MultiplayerPeer = null;

    }

    [Rpc(CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void LoadGame()
    {
        GD.Print("LoadGame()");
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void PlayerLoaded()
    {
        if (Multiplayer.IsServer())
        {
            if (Players.Count == MAX_CONNECTIONS)
            {
                GD.Print("All players loaded");
            }
        }
    }

    public void ReturnToMainMenu()
    {
        EmitSignal("ReturnToMenu");
        Input.MouseMode = Input.MouseModeEnum.Visible;
        if (playerInfo.isServer)
        {
            foreach ((long key, PlayerInfo value) in Players)
            {
                if (key == 1) continue;
                Multiplayer.MultiplayerPeer.DisconnectPeer((int)key);
            }
            Multiplayer.MultiplayerPeer.Close();
            removeMPPeer();
            Players = new Dictionary<long, PlayerInfo>();
            lobbyInfo.Visible = false;
            menu.Visible = false;
        }
        else
        {
            // disconnect from server if still connected
            if (Multiplayer.HasMultiplayerPeer())
            {
                GD.Print("still connected");
                Multiplayer.MultiplayerPeer.Close();
                removeMPPeer();
            }
            Players = new Dictionary<long, PlayerInfo>();
            lobbyInfo.Visible = false;
        }
    }

    private void onLeaveButtonPressed()
    {
        ReturnToMainMenu();
    }

    private void onPlayerConnected(long peerId)
    {
        RpcId(peerId, "registerPlayer", playerInfo.encode());
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void registerPlayer(Godot.Collections.Dictionary PlayerInfo)
    {
        PlayerInfo newPlayerInfo = new PlayerInfo(0, "", false);
        newPlayerInfo.decode(PlayerInfo);
        long newPlayerId = Multiplayer.GetRemoteSenderId();
        Players[newPlayerId] = newPlayerInfo;
        EmitSignal("PlayerConnected", newPlayerInfo);
        updateInfo();
    }

    private void onPlayerDisconnected(long peerID)
    {
        Players.Remove(peerID);
        EmitSignal("PlayerDisconnected", peerID);
    }

    private void onConnectedOk()
    {
        playerInfo.peerID = Multiplayer.GetUniqueId();
        playerInfo.isServer = Multiplayer.IsServer();
        Players[playerInfo.peerID] = playerInfo;
        EmitSignal("PlayerConnected", playerInfo);
    }

    private void onConnectedFail()
    {
        // TODO: notify user
        ReturnToMainMenu();
    }

    private void onServerDisconnected()
    {
        GD.Print("Disconnected from server");
        Multiplayer.MultiplayerPeer = null;
        Players = new Dictionary<long, PlayerInfo>();
        EmitSignal("ServerDisconnected");
        ReturnToMainMenu();
    }

    private void onStartGameButtonPressed()
    {
        EmitSignal("LobbyGameStarted");
        menu.Visible = false;
        lobbyInfo.Visible = true;
        chat.Visible = true;
    }
}
