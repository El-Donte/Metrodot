using Godot;
using System;

public partial class Saw : Area2D
{
	Path2D path;
	PathFollow2D follow;

	[ExportGroup("movement")]
	[Export]int speed = 200;
	[Export]int offset = 0;
	[Export]bool forward = true;

    public override void _Ready()
    {
		path = (Path2D)GetChildren()[2];

		follow = new PathFollow2D();
		path.AddChild(follow);
		follow.Progress = offset;
    }

    public override void _Process(double delta)
    {
		int direction = forward ? 1 : -1;
		follow.Progress += speed * (float)delta * direction;
		Position = follow.Position;
	}

	void OnBodyEntered(Node2D body)
	{
		if (body is Entity entity) 
		{
			Player player = body as Player;
			entity.Hit(10,player.GetMaterials());
		}
	}
}
