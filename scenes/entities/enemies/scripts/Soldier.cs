using Godot;
using System;
using System.Collections.Generic;

public partial class Soldier : Entity
{
	int xDirection = 1;
	int speed = Global.enemy["soldier"]["speedSoldier"];
	int speedModifier = 1;
	bool attack = false;
    Player player;
    [Export] Sprite2D sprite;
    [Export] CollisionShape2D collision;
    [Export] CollisionShape2D collision2;
    [Export] AnimationPlayer animPlayer;
    [Export] RayCast2D right;
    [Export] RayCast2D left;

    public override void _Ready()
    {
        player = GetTree().GetFirstNodeInGroup("Player") as Player;
        health = Global.enemy["soldier"]["healthSoldier"];
    }

    public override void _Process(double delta)
    {
        if (health > 0)
        {
            //Move
            Move();
            //check for clifs
            CheckClifs();
            CheckPlayerDistance();
            //animate
            Animate();
            MoveAndSlide();
        }
    }

    void CheckPlayerDistance()
    {
        if(Position.DistanceTo(player.Position) < 120)
        {
            attack = true;
            speedModifier = 0;

        }
        else
        {
            attack = false;
            speedModifier = 1;
        }
    }

    void Move()
    {
        Vector2 _velocity = Velocity;
        _velocity.X = xDirection * speed * speedModifier;
        Velocity = _velocity;
    }

    void Animate()
    {
        sprite.FlipH = xDirection < 0;
        if (attack)
        {
            string side = "right";
            var difference = (player.Position - Position).Normalized();
            sprite.FlipH = difference.X < 0;
            if(difference.Y < -0.5 && Mathf.Abs(difference.X) < 0.4)
            {
                side = "up";
            }
            if (difference.Y > 0.5 && Mathf.Abs(difference.X) < 0.4)
            {
                side = "down";
            }
            animPlayer.CurrentAnimation = "shoot_" + side;
            return;
        }
        animPlayer.CurrentAnimation = xDirection != 0 ? "run" : "idle";
    }

    void CheckClifs()
    {
        if(xDirection > 0 && !right.IsColliding())
        {
            xDirection = -1;
        }
        if (xDirection < 0 && !left.IsColliding())
        {
            xDirection = 1;
        }
    }

    void OnWallCheck(Node2D body)
    {
        xDirection *= -1;
    }

    void TringgerAttack()
    {
        var dir = (player.Position - Position).Normalized();
        EmitSignal(SignalName.Shoot, Position + dir * 20, dir, (int)Global.Guns.AK);
    }

    public List<Material> GetMaterials()
    {
        return new List<Material> { sprite.Material };
    }

    void DisableCollisions()
    {
        collision.Disabled = true; 
        collision2.Disabled = true;
    }

    protected override void TriggerDeath()
    {
        speedModifier = 0;
        animPlayer.CurrentAnimation = "death";
        CallDeferred("DisableCollisions");
    }
}
