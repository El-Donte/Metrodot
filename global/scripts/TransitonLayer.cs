using Godot;
using System;

public partial class TransitonLayer : CanvasLayer
{
	[Export] ColorRect colorRect;
	public override void _Ready()
	{
		colorRect.Modulate = new Color(0,0,0,0);
		colorRect.Visible = false;
	}

	void ChangeScence(string scence)
	{
        colorRect.Visible = true;
        Player player =(Player)GetTree().GetFirstNodeInGroup("Player");
		player.BlockMovement();
		var tween = CreateTween();
		tween.TweenProperty(colorRect, "modulate", new Color(0, 0, 0, 1), 0.5);
		tween.TweenCallback(Callable.From(() => OpenScene(scence)));
        tween.TweenProperty(colorRect, "modulate", new Color(0, 0, 0, 0), 0.5);
    }

    void OpenScene(string scence)
	{
		GetTree().ChangeSceneToFile(scence);
	}
}
