using Godot;
using System;
using System.Collections.Generic;
public partial class Demo : Node3D
{
    [Export]
    public PackedScene PlayerPrefab;

    Lobby lobby;
    PlayerSpawnPoint PlayerSpawner;
    bool PlayersReady = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        lobby = GetNode<Lobby>("/root/Lobby");
        PlayerSpawner = GetNode<PlayerSpawnPoint>("PlayerSpawnPoint");
        GD.Print("Demo _ready");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!PlayersReady)
        {
            SpawnPlayers();
        }
    }

    [Rpc]
    private void SpawnPlayers()
    {
        // TODO: fast return instead
        if (Multiplayer.IsServer())
        {
            if (!OS.HasFeature("dedicated_server"))
            {
                PlayerSpawner.SpawnPlayer(1);
            }
            foreach (KeyValuePair<long, PlayerInfo> player in lobby.Players)
            {
                if (player.Key != Multiplayer.GetUniqueId())
                {
                    PlayerSpawner.SpawnPlayer((int)player.Key);
                }
            }
        }
        PlayersReady = true;
    }

}
