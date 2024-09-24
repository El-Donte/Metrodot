using Godot;
using System;
using System.Collections.Generic;

public partial class Monster : Entity
{
    [Export] Vector2I limits;
    [Export] Timer moveTimer;
    [Export] AnimationPlayer animPLayer;
    [Export] Timer attackTimer;
    [Export] Sprite2D sprite;
         
    Player player;
    Camera2D playerCamera;
    float camSize;
    Vector2 yRange = new Vector2(-50, 50);
    float yOffset;
    RandomNumberGenerator random = new RandomNumberGenerator();

    [Signal]
    public delegate void DetonateEventHandler(Vector2 position);

    public override void _Ready()
    {
        player = GetTree().GetFirstNodeInGroup("Player") as Player;
        playerCamera = player.GetCam();
        health = Global.enemy["monster"]["healthMonster"];
        camSize = playerCamera.GetViewportRect().Size.X / playerCamera.Zoom.X;
    }

    public override void _Process(double delta)
    {
        var x = player.Position.X + camSize / 2 - 15;
        x = MathF.Max(limits.X,MathF.Min(limits.Y,x));
        var y = player.Position.Y + yOffset;
        Position = new Vector2(x,y);
    }

    void OnAttackTimerTimeout()
	{
        animPLayer.CurrentAnimation = "attack";
        attackTimer.WaitTime = random.RandfRange(0.5f,2.0f);
        attackTimer.Start();
    }

    void TriggerAttack()
    {
        var optionIndex = random.RandiRange(0,GetNode<Node2D>("BulletOptions").GetChildCount() - 1);
        var selected = GetNode< Node2D > ("BulletOptions").GetChildren()[optionIndex];
        foreach (Marker2D marker in selected.GetChildren()) 
        {
            EmitSignal(SignalName.Shoot, marker.GlobalPosition, Vector2.Left, (int)Global.Guns.Monster);
        }
    }

    protected override void TriggerDeath()
    {
        attackTimer.Stop();
        moveTimer.Stop();
        animPLayer.CurrentAnimation = "death";
    }

    void Explode()
    {
        int randX = random.RandiRange((int)GlobalPosition.X-20,(int)GlobalPosition.X+20);
        int randY = random.RandiRange((int)GlobalPosition.Y-20,(int)GlobalPosition.Y+20);
        EmitSignal(SignalName.Detonate,new Vector2(randX, randY));
    }

    void Finish()
    {
        ((ColorRect)GetTree().GetFirstNodeInGroup("finish")).Color = new Color(0f, 0f, 0f, 1f);
        ((ColorRect)GetTree().GetFirstNodeInGroup("finish")).GetChild<Label>(0).Visible = true;
    }

    void Exit()
    {
        GetTree().Quit();
    }

    void OnMoveTimerTimeout()
	{
        var tween = CreateTween();
        tween.TweenProperty(this, "yOffset", random.RandfRange(yRange.X, yRange.Y), 0.6);
        tween.TweenCallback(Callable.From(() => moveTimer.Start()));
	}

    void ReturnToIdle()
    {
        animPLayer.CurrentAnimation = "idle";
    }

    public List<Material> GetMaterials()
    {
        return new List<Material> { sprite.Material };
    }
}
