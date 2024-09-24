using Godot;
using System;

public partial class Crosshair : Sprite2D
{
    int y_offset = 6;
    public void Update(Vector2 direction,int distance, bool crouch)
	{
		int crouch_offset = crouch ? y_offset : 0;
		Position = direction * distance + new Vector2(0,crouch_offset);
	}
}
