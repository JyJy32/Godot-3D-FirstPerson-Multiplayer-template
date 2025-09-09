using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export]
    public float SPEED = 5.0f;
    [Export]
    public float JUMP_VEL = 4.5f;
    [Export]
    public int playerID = 0;
    public bool inMenu = false;
    CameraGimbal Camera;
    GameMenu menu;
    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    // input
    bool Jumping = false;
    float VerticalVel = 0; // postive going up, negative going down
    // Vector2 Direction = Vector2.Zero;

    PlayerInput playerInput;


    public override void _EnterTree()
    {
        base._EnterTree();
        if (playerID == 0) return;
        //SetMultiplayerAuthority(playerID);
    }

    public override void _Ready()
    {
        menu = GetNode<GameMenu>("GameMenu");
        Camera = GetNode<CameraGimbal>("CameraGimbal");
        playerInput = GetNode<PlayerInput>("PlayerInput");

        if (playerID == Multiplayer.GetUniqueId())
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            Camera.SetCameraActive(true);
            MeshInstance3D mesh = GetNode<MeshInstance3D>("MeshInstance3D");
            mesh.SetLayerMaskValue(11, true);
            mesh.SetLayerMaskValue(10, false);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = new Vector3();
        if (IsOnFloor()) VerticalVel = 0;
        // FIXME: this IsOnFloor check might be reduntant
        if (playerInput.Jumping && IsOnFloor())
        {
            VerticalVel = JUMP_VEL;
            playerInput.Jumping = false;
        }

        velocity.Y = VerticalVel;

        if (!IsOnFloor()) VerticalVel -= gravity * (float)delta;

        Vector2 direction = playerInput.Direction;
        if (direction != Vector2.Zero)
        {
            velocity.X += direction.X * SPEED;
            velocity.Z += direction.Y * SPEED;
        }
        else
        {
            velocity.X = 0;
            velocity.Z = 0;
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
