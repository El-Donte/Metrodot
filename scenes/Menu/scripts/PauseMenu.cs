using Godot;
using System;

public partial class PauseMenu : Control
{
    [Export] Button start;
    [Export] Button exit;
    [Export] Button restart;
    public override void _Ready()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("RESET");
    }

    void EnableButtons()
    {
        start.Disabled = true;
        exit.Disabled = true;
        restart.Disabled = true;
    }

    void DisableButtons()
    {
        start.Disabled = false;
        exit.Disabled = false;
        restart.Disabled = false;
    }

    public override void _Process(double delta)
    {
        CheckEsc();
    }
    void Pause()
    {
        MouseMode();
        DisableButtons();
        GetTree().Paused = true;
        GetNode<AnimationPlayer>("AnimationPlayer").Play("blur");
    }

    void Continue()
    {
        MouseMode();
        EnableButtons();
        GetTree().Paused = false;
        GetNode<AnimationPlayer>("AnimationPlayer").PlayBackwards("blur");
    }

    void CheckEsc()
    {
        if (Input.IsActionJustPressed("exit") && !GetTree().Paused) 
        {
            Pause();
        }
        else if (Input.IsActionJustPressed("exit") && GetTree().Paused)
        {
            Continue();
        }
    }

    void OnContinuePressed()
    {
        Continue();
    }

    void OnRestartPressed()
    {
        Continue();
        GetTree().ReloadCurrentScene();
    }
    void OnExitPressed()
    {
        GetTree().Quit();
    }

    void MouseMode()
    {
        if(DisplayServer.MouseGetMode() == DisplayServer.MouseMode.Hidden)
        {
            DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);
        }
        else
        {
            DisplayServer.MouseSetMode(DisplayServer.MouseMode.Hidden);
        }
    }
}
