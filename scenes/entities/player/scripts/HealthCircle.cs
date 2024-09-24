using Godot;
using System;

public partial class HealthCircle : Sprite2D
{
    Player player;
    public override void _Ready()
    {
        player = GetTree().GetFirstNodeInGroup("Player") as Player;
        ((ShaderMaterial)Material).SetShaderParameter("alpha", 0.0);
    }

    public void Update(int health)
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "material:shader_parameter/alpha", 1.0, 0.1);
        tween.TweenProperty(this, "material:shader_parameter/progress", health / 200.0, 0.2);
        tween.TweenProperty(this, "material:shader_parameter/alpha", 0.0, 0.4);
    }

}
