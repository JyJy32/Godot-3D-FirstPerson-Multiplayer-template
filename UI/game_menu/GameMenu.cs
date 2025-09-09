using Godot;
using System;

public partial class GameMenu : CanvasLayer
{
    [Signal]
    public delegate void MenuOpenedEventHandler();
    [Signal]
    public delegate void MenuClosedEventHandler();

    Lobby lobby;

    public override void _Ready()
    {
        lobby = GetNode<Lobby>("/root/Lobby");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            Visible = !Visible;
            if (Visible)
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
                EmitSignal("MenuOpened");
            }
            else
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }
    }

    private void onContinuePressed()
    {
        Visible = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    private void onReturnToMenuPressed()
    {
        lobby.ReturnToMainMenu();
    }

    private void onQuitPressed()
    {
        GetTree().Quit();
    }

}
