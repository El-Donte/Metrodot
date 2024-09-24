using Godot;
using System;

public partial class Bullet : Area2D
{
    [Export] Sprite2D sprite;
    [Export] PointLight2D light;
    [Export] CollisionShape2D collisionShape;
    [Export] public AudioStreamPlayer2D  audio;

    int speed;
    int damage;
    bool explosive = false;
    int shotgunRange = 100;
    int bulletType = 0;
    Vector2 direction;

    [Signal]
    public delegate void DetonateEventHandler(Vector2 position);

    public void Setup(Vector2 pos,Vector2 dir,int bulletType)
    {
        audio.Stream = (AudioStreamWav)Global.bulletSound[bulletType];
        audio.VolumeDb = -12;
        audio.Play();
        Position = pos;
        direction = dir.Normalized();
        if (bulletType == 0) //ak
        {
            collisionShape.Disabled = false;
            sprite.Visible = true;
            light.Visible = true;
            ChangeSettings(200, 20, GD.Load("res://graphics/guns/projectiles/default.png") as Texture2D);
        }
        if (bulletType == 1) //shotgun
        {
            collisionShape.Disabled = false;
            sprite.Visible = false;
            light.Visible = false;
            damage = 30;
            foreach(Entity enemy in GetTree().GetNodesInGroup("Enemies"))
            {
                var bulletAngle = Mathf.RadToDeg(dir.Angle());
                var enemyAngle = Mathf.RadToDeg((enemy.Position - pos).Angle());
                if(Position.DirectionTo(enemy.Position).X < shotgunRange && Mathf.Abs(bulletAngle - enemyAngle) < 90)
                {
                    GetBody(enemy);
                }
            }
        }
        if (bulletType == 2) //rocket
        {
            collisionShape.Disabled = false;
            sprite.Visible = true;
            light.Visible = true;
            explosive = bulletType==(int)Global.Guns.ROCKET;
            ChangeSettings(120, 60, GD.Load("res://graphics/guns/projectiles/large.png") as Texture2D);
        }
        if (bulletType == 3)//Monster
        {
            collisionShape.Disabled = false;
            sprite.Visible = true;
            light.Visible = true;
            ChangeSettings(200, 30, GD.Load("res://graphics/guns/projectiles/60.png") as Texture2D);
        }

    }

    public override void _Process(double delta)
    {
        Position += direction * 100 * (float)delta; 
    }

    void ChangeSettings(int _speed, int _damage, Texture2D texture) 
    {
        speed = _speed;
        damage = _damage;
        sprite.Texture = texture;

    }
    void OnBodyEntered(Node2D body)
    {
        EmitSignal(SignalName.Detonate, Position);
        if(body is Entity entity)
        {
            if(body is Soldier)
            {
                Soldier soldier = (Soldier)body;
                entity.Hit(damage, soldier.GetMaterials());
            }
            if (body is Player)
            {
                Player player = (Player)body;
                entity.Hit(damage, player.GetMaterials());
            }
            if (body is Monster)
            {
                Monster monster = (Monster)body;
                entity.Hit(damage, monster.GetMaterials());
            }

        }
        QueueFree();
    }

    void GetBody(Entity entity)
    {
        if (entity is Soldier)
        {
            Soldier soldier = (Soldier)entity;
            GD.Print(soldier);
            entity.Hit(damage, soldier.GetMaterials());
        }
        if (entity is Player)
        {
            Player player = (Player)entity;
            entity.Hit(damage, player.GetMaterials());
        }
        if (entity is Monster)
        {
            Monster monster = (Monster)entity;
            entity.Hit(damage, monster.GetMaterials());
        }
    }

    void OnKillTimerTimeout()
    {
        QueueFree();
    }

}
