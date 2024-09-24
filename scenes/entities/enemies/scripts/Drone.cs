using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

public partial class Drone : Entity
{
    [Export] AnimatedSprite2D sprite;

	bool active = false;
	int speed;
	int damage;
    Player player;

    [Signal]
    public delegate void DetonateEventHandler(Vector2 position);
    public override void _Ready()
    {
        player =(Player)GetTree().GetFirstNodeInGroup("Player");
        health = Global.enemy["drone"]["healthDrone"];
        speed = Global.enemy["drone"]["speedDrone"];
        damage = Global.enemy["drone"]["damageDrone"];
    }

    public override void _Process(double delta)
    {
        if (active)
        {
            Vector2 _velocity = Velocity;
            var direction = (player.Position - Position).Normalized();
            _velocity = direction * speed;
            Velocity = _velocity;
            MoveAndSlide();
        }
    }

    void OnPlayerDetection(Node2D body)
    {
        active = true;
    }

    void OnCollisionPlayerDetection(Node2D body) 
    {
        if (body != this)
        {
            EmitSignal(SignalName.Detonate, GlobalPosition);
            QueueFree();
            if (body is Player)
            {
                Player player = (Player)body;
                player.Hit(damage, player.GetMaterials());
            }
        }
    }

    public List<Material> GetMaterials()
    {
        return new List<Material> { sprite.Material };
    }

    protected override void TriggerDeath()
    {
        EmitSignal(SignalName.Detonate,GlobalPosition);
        QueueFree();
    }
}
