using Godot;
using System;

public partial class PlayerInput : MultiplayerSynchronizer
{
    [Export]
    public Vector2 Direction;
    [Export]
    public bool Jumping = false;
    Player player;
    CameraGimbal Camera;
    bool inMenu = false;
    public override void _Ready()
    {
        player = GetParent<Player>();
        Camera = player.GetNode<CameraGimbal>("CameraGimbal");
        player.GetNode<GameMenu>("GameMenu").MenuOpened += onMenuOpened;
        player.GetNode<GameMenu>("GameMenu").MenuClosed += onMenuClosed;

        SetProcess(player.playerID == Multiplayer.GetUniqueId());
        SetProcessInput(player.playerID == Multiplayer.GetUniqueId());
    }

    public override void _EnterTree()
    {
        CallDeferred("set_multiplayer_authority", GetParent<Player>().playerID);
    }


    public override void _Process(double delta)
    {
        Vector2 dir = Input.GetVector("left", "right", "forward", "back");

        Quaternion cameraRotation = Quaternion.FromEuler(new Vector3(0, Camera.getLookDirection(), 0));
        Vector3 direction = new Vector3(dir.X, 0, dir.Y);
        Vector3 worldDirection = cameraRotation * direction;
        worldDirection = worldDirection.Normalized();
        Direction.X = worldDirection.X;
        Direction.Y = worldDirection.Z;

        if (Input.IsActionJustPressed("jump") && !Jumping && player.IsOnFloor())
        {
            // the server needs this knowledge it will change the position of the player and sync that
            RpcId(1, "jump");
        }
    }
    public override void _Input(InputEvent @event)
    {
        if (inMenu) return;
        float sensitivity = 0.1f;
        if (@event is InputEventMouseMotion)
        {
            InputEventMouseMotion mouseEvent = @event as InputEventMouseMotion;
            Camera.SetRotateY(-mouseEvent.Relative.X * sensitivity);
            Camera.SetRotateX(-mouseEvent.Relative.Y * sensitivity);
        }

    }

    [Rpc(CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
    public void jump()
    {
        Jumping = true;
    }

    private void onMenuOpened()
    {
        inMenu = true;
    }

    private void onMenuClosed()
    {
        inMenu = false;
    }
}
