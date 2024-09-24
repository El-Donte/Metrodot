using Godot;
using System;

public partial class Menu : Control
{
    BgMusic bgMusic;
    public override void _Ready()
    {
        bgMusic = GetNode<BgMusic>("/root/BgMusic");
    }
    void OnStartPressed()
    {
        DisplayServer.MouseSetMode(DisplayServer.MouseMode.Hidden);
        bgMusic.play();
        GetTree().ChangeSceneToFile("res://scenes/levels/scenes/outside.tscn");
    }
    void OnExitPressed()
    {
        GetTree().Quit();
    }
}
