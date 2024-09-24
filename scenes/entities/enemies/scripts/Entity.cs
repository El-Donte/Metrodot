using Godot;
using System;
using System.Collections.Generic;

public partial class Entity : CharacterBody2D
{
    [Signal]
    public delegate void ShootEventHandler(Vector2 position,Vector2 direction,int bulletType);

    [Export] Timer InvurTimer;

    List<Material> materials;

    public int health = 100;
    public void Hit(int damage,List<Material> materials)
    {
        if (InvurTimer.TimeLeft == 0) 
        {
            health -= damage;
            InvurTimer.Start();
            if (IsInGroup("Player"))
            {
                HealthCircle healthCircle = GetTree().GetFirstNodeInGroup("HealthCircle") as HealthCircle;
                healthCircle.Update(health);
                ((AudioStreamPlayer2D)GetNode("AudioStreamPlayer2D")).Play();
            }
            Flash(materials);
        }
        if (health <= 0) 
        {
            TriggerDeath();
        }
    }

    void Flash(List<Material> materials)
    {
        this.materials = materials;
        var tween = CreateTween();
        tween.TweenMethod(Callable.From<float>(SetFlashValue), 0.0f, 1.0f, 0.4).SetTrans(Tween.TransitionType.Quad);
        tween.TweenMethod(Callable.From<float>(SetFlashValue), 1.0f, 0.0f, 0.4).SetTrans(Tween.TransitionType.Quad);
    }

    void SetFlashValue(float value) 
    {
        foreach(Material material in materials)
        {
            ((ShaderMaterial)material).SetShaderParameter("Progress",value);
        }
    }

    //public virtual void Setup(Entity entity)
    //{
    //    if (this.IsInGroup("Enemies"))
    //    {
    //        GetNode("Main/Entities").AddChild(entity);
    //        Position = entity.Position;
    //        Velocity = entity.Velocity;
    //        health = entity.health;
    //        //if (entity is Soldier)
    //        //{
    //        //    GetNode("Main/Entities").AddChild(this);
    //        //    Soldier soldier = entity as Soldier;
    //        //    Position = soldier.Position;
    //        //    Velocity = soldier.Velocity;
    //        //    health = soldier.health;
    //        //}
    //        //if (entity is Monster)
    //        //{
    //        //    Monster monster = entity as Monster;
    //        //    Position = monster.Position;
    //        //    Velocity = monster.Velocity;
    //        //    health = monster.health;
    //        //}
    //        //if (entity is Drone)
    //        //{
    //        //    Drone drone = entity as Drone;
    //        //    Position = drone.Position;
    //        //    Velocity = drone.Velocity;
    //        //    health = drone.health;
    //        //}
    //    }
    //}

    protected virtual void TriggerDeath() { }
}
