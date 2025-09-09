using Godot;
using System;

public partial class PlayerSpawner : MultiplayerSpawner
{
    public override void _Ready()
    {
        base._Ready();
        SetMultiplayerAuthority(1);
    }


	public void Spawn(Node scene)
	{
		GetNode(SpawnPath).CallDeferred("add_child", scene, true);
        
		//AddChild(scene);
	}
}
