using Godot;

public partial class CameraGimbal : Node3D
{

    public Camera3D Camera;
    Node3D InnerGimbal;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Camera = GetNode<Camera3D>("InnerGimbal/Camera");
        InnerGimbal = GetNode<Node3D>("InnerGimbal");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void SetCameraActive(bool active)
    {
        Camera.Current = active;
    }

    public void SetRotateX(float deg)
    {
        InnerGimbal.RotateObjectLocal(Vector3.Right, Mathf.DegToRad(deg));
    }

    public void SetRotateY(float deg)
    {
        RotateObjectLocal(Vector3.Up, Mathf.DegToRad(deg));
    }

    public float getLookDirection()
    {
        return Rotation.Y;
    }
}
