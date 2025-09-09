using Godot;
using System;

public partial class PlayerSpawnPoint : Node3D
{
    [Export]
    PackedScene[] packedScenes = [];
    PlayerSpawner playerSpawner;
    Node players;

    public override void _Ready()
    {
        if (Multiplayer.IsServer())
        {
            GetNode<Lobby>("/root/Lobby").PlayerDisconnected += RemovePlayer;
        }

        playerSpawner = GetNode<PlayerSpawner>("PlayerSpawner");
        foreach (PackedScene scene in packedScenes)
        {
            playerSpawner.AddSpawnableScene(scene.ResourcePath);
        }
        players = GetNode("Players");
    }

    [Rpc]
    public void SpawnPlayer(int id)
    {
        if (packedScenes.Length == 0)
        {
            throw new NoPlayerPrefabSetException();
        }
        // TODO: test out of this is ok for sub classes of player
        Player player = packedScenes[0].Instantiate<Player>();
        player.playerID = id;
        player.Position = Position; // default spawn @ PlayerSpawnPoint
        player.UniqueNameInOwner = true;
        GD.Print("Adding player: ", id);
        playerSpawner.Spawn(player);
    }

    [Rpc]
    public void SpawnPlayer(int id, Vector3 position) { }

    [Rpc]
    public void SpawnPlayer(int id, int variant)
    {
        if (packedScenes.Length <= variant)
        {
            throw new VaraintOutOfBoundsException();
        }
    }

    [Rpc]
    public void SpawnPlayer(int id, int variant, Vector3 position)
    {
        if (packedScenes.Length <= variant)
        {
            throw new VaraintOutOfBoundsException();
        }
    }

    public void RemovePlayer(int id)
    {
        foreach (Player player in players.GetChildren())
        {
            if (player.playerID == id)
            {
                players.RemoveChild(player);
                break;
            }
        }
    }

}

class NoPlayerPrefabSetException: Exception {}
class VaraintOutOfBoundsException: Exception {}