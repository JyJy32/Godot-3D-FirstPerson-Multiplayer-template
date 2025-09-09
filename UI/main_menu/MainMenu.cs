using Godot;
using System;

public partial class MainMenu : CanvasLayer
{
    [Signal]
    public delegate void playerCreatedEventHandler(string name);
    [Signal]
    public delegate void startServerEventHandler();
    [Signal]
    public delegate void joinGameEventHandler(string address);

    // pages
    Control playerInfo;
    Control createPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        playerInfo = GetNode<Control>("Menu/PlayerInfo");
        createPlayer = GetNode<Control>("CreatePlayer");
        LineEdit name_input = createPlayer.GetNode<LineEdit>("VBoxContainer/NameInput");
        name_input.GrabFocus();

        if (OS.IsDebugBuild())
        {
            Random rand = new Random();
            name_input.Text = rand.NextInt64().ToString();
        }

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }

    public override void _Input(InputEvent @event)
    {
        if (createPlayer.Visible && @event.IsActionPressed("ui_accept"))
        {
            if (createPlayer.GetNode<LineEdit>("VBoxContainer/NameInput").Text != "")
            {
                onPlayerCreateButtonPressed();
                createPlayer.Hide();
            }
        }
    }

    public void hideMenu()
    {
        Visible = false;
    }

    private void onPlayerCreateButtonPressed()
    {
        string name = GetNode<LineEdit>("CreatePlayer/VBoxContainer/NameInput").Text;
        if (name == "")
        {
            return;
        }
        EmitSignal("playerCreated", name);
        playerInfo.GetNode<RichTextLabel>("VBoxContainer/Name").Text = name;
        createPlayer.Hide();
    }

    private void onStartServerButtonPressed()
    {
        EmitSignal("startServer");
        hideMenu();
    }

    private void onJoinButtonPressed()
    {
        EmitSignal("joinGame", "");
        hideMenu();
    }

}
