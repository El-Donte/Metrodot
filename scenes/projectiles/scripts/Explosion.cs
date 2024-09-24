using Godot;
using System;

public partial class Explosion : AnimatedSprite2D
{
	public override void _Ready()
	{
		((AudioStreamPlayer2D)GetNode("AudioStreamPlayer2D")).Play();
		
	}
	void OnAnimationFinished()
	{
		QueueFree();
	}
}
