using Godot;
using System;

public partial class PlayerGraphics : Node
{
    [Export]
    public AnimatedSprite2D legs;
    [Export]
    public Sprite2D torso;
    [Export]
    AnimationTree tree;
    [Export] GpuParticles2D gpuParticles2D;
    int y_offset = 6;
    public void UpdateLegs(Vector2 direction,bool isOnFloor, bool crouch)
    {
        //flip
        if(direction.X != 0)
        {
            legs.FlipH = direction.X < 0;
        }

        //state
        string state = !crouch ? "idle" : "crouch";
        if(isOnFloor && direction.X != 0 && !crouch)
        {
            state = "run";
        }

        if (!isOnFloor)
        {
            state = "jump";
        }

        legs.Animation = state;
    }

    public void UpdateTorso(Vector2 direction, bool crouch,Global.Guns CurrentGun)
    {
        tree.SelectedGuns = CurrentGun;
        var stateMachine = tree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
        if (tree.SelectedGuns == Global.Guns.SHOTGUN) 
        {
            stateMachine.Travel("ShotGun");
        }
        if(tree.SelectedGuns == Global.Guns.AK)
        {
            stateMachine.Travel("AK");
        }
        if(tree.SelectedGuns == Global.Guns.ROCKET)
        {
            stateMachine.Travel("Rocket");
        }
        
        Vector2 position = torso.Position;
        position.Y = crouch ?  y_offset : 0;
        torso.Position = position;
        tree.Set("parameters/AK/blend_position", direction);
        tree.Set("parameters/ShotGun/blend_position", direction);
        tree.Set("parameters/Rocket/blend_position", direction);
    }

    public void DashParticles(Vector2 direction)
    {
        gpuParticles2D.ProcessMaterial.Set("direction",direction);
        gpuParticles2D.Restart();
    }
}