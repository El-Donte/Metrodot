using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

public partial class Player : Entity
{
	[ExportGroup("move")]
	[Export] int speed = 200;
	[Export] int acceleration = 700;
	[Export] int friction = 900;
	[Export] Timer DashCooldown;
	[Export] Timer Coyote;
	[Export] Timer JumpBuffer;
	Vector2 direction = Vector2.Zero;
	bool canMove = true;
	bool canDash = false;
	bool crouch = false;
	float dashCooldown = 0.5f;
	

	[ExportGroup("jump")]
	[Export] int jumpStrenght = 300;
	[Export] int gunJumpStrenght = 180;
	[Export] int gravity = 600;
	[Export] float terminalVelocity = 500f;
	bool jump = false;
	bool gunJump = false;
	bool fasterFall = false;
	int gravityMultiplier = 1;

	[ExportGroup("Gun")]
	[Export] Crosshair crosshair;
	[Export] int crosshairDistance = 20;
	[Export] Timer AkReload;
	[Export] Timer ShotgunReload;
	[Export] Timer RocketReload;
    [Export(PropertyHint.Range, "0.2,2.0")]
	float akCooldown = 0.5f;
    [Export(PropertyHint.Range, "0.2,2.0")]
    float shotgunCooldown = 1.2f;
    [Export(PropertyHint.Range, "0.2,2.0")]
    float rocketCooldown = 1.5f;
    Vector2 aimDirection = Vector2.Right;
    Global.Guns currentGun = Global.Guns.AK;

	[ExportGroup("Graphics")]
	[Export] PlayerGraphics graphics;
	[Export] GpuParticles2D particles;
	[Export] Camera2D camera;
    public override void _Ready()
	{
		DashCooldown.WaitTime = dashCooldown;
		RocketReload.WaitTime = rocketCooldown;
		AkReload.WaitTime = akCooldown;
		ShotgunReload.WaitTime = shotgunCooldown;
		health = 200;
	}

	public override void _Process(double delta)
	{
        ApplyGravity(delta);
		if (canMove) 
		{
			GetInput();
			ApplyMovement(delta);
			Animate();
			MoveAndSlide();
		}


	}

	void Animate()
	{
		crosshair.Update(aimDirection, crosshairDistance,crouch);
		graphics.UpdateLegs(direction,IsOnFloor(),crouch);
		graphics.UpdateTorso(aimDirection,crouch,currentGun);
	}

	void GetInput()
	{
		var velocity = Velocity;
		direction.X = Input.GetAxis("left", "right");

		//jump
		if (Input.IsActionJustPressed("jump"))
		{
			if (IsOnFloor() || Coyote.TimeLeft != 0)
			{
				jump = true;
			}

			if (velocity.Y > 0 && !IsOnFloor())
			{
				JumpBuffer.Start();
			}
		}

		if (Input.IsActionJustReleased("jump") && !IsOnFloor() && velocity.Y < 0)
		{
			fasterFall = true;
		}

		//dash
		if (Input.IsActionJustPressed("dash") && velocity.X != 0 && !(DashCooldown.TimeLeft != 0))
		{
			canDash = true;
			DashCooldown.Start();
		}

		crouch = Input.IsActionPressed("crouch") && IsOnFloor();
		//aim
		var aim_mouse = GetLocalMousePosition().Normalized();
		if(aim_mouse.Length() > 0.5)
		{
			aimDirection = new Vector2(Mathf.Round(aim_mouse.X), Mathf.Round(aim_mouse.Y));
		}

		if (Input.IsActionJustPressed("switch"))
		{
			currentGun = (Global.Guns)((int)(currentGun + 1) % 3);
		}

		if (Input.IsActionJustPressed("1"))
		{
			currentGun = (Global.Guns)0;
		}
        if (Input.IsActionJustPressed("2"))
        {
            currentGun = (Global.Guns)1;
        }
        if (Input.IsActionJustPressed("3"))
        {
            currentGun = (Global.Guns)2;
        }


        //shot
        if (Input.IsActionPressed("shoot"))
		{
			ShootGun();
		}

		Velocity = velocity;
	}

	void ShootGun()
	{
		var pos = Position + aimDirection * crosshairDistance;
		pos = !crouch ? pos : pos + new Vector2(0, 6);
		if (currentGun == Global.Guns.AK && AkReload.TimeLeft == 0) 
		{
			EmitSignal(SignalName.Shoot, pos, aimDirection,(int)currentGun);
			AkReload.Start();
		}
        if (currentGun == Global.Guns.ROCKET && RocketReload.TimeLeft == 0)
        {
            EmitSignal(SignalName.Shoot, pos, aimDirection, (int)currentGun);
            RocketReload.Start();
        }
        if (currentGun == Global.Guns.SHOTGUN && ShotgunReload.TimeLeft == 0)
        {
            EmitSignal(SignalName.Shoot, pos, aimDirection, (int)currentGun);
            ShotgunReload.Start();
			//shotgun particles
			particles.Position = crosshair.Position;
			particles.ProcessMaterial.Set("direction", aimDirection);
			particles.Emitting = true;
			//shotgun jump
			if(aimDirection.Y == 1 && Velocity.Y >= 0)
			{
				gunJump = true;
			}
        }
    }

	void ApplyMovement(double delta)
	{
		var velocity = Velocity;
		if (direction.X != 0)
		{
			velocity.X = Mathf.MoveToward(velocity.X, direction.X * speed, acceleration * (float)delta);
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, friction * (float)delta);
		}

		if (crouch)
		{
			velocity.X = 0;
		}

		if (jump && IsOnFloor())
		{
			velocity.Y = -jumpStrenght;
			jump = false;
			fasterFall = false;
		}

        //gun jump
        if (gunJump)
        {
            velocity.Y = -gunJumpStrenght;
            gunJump = false;
            fasterFall = false;
        }


        bool on_floor = IsOnFloor();
		if (on_floor && !IsOnFloor() && velocity.Y >= 0)
		{
			Coyote.Start();
			GD.Print("fall");
		}

		if (canDash)
		{
			((AudioStreamPlayer2D)GetNode("DashSound")).Play();
			canDash = false;
			graphics.DashParticles(direction);
			Tween can_dash_tween = CreateTween();
			can_dash_tween.TweenProperty(this, "velocity:x", velocity.X + direction.X * 600, 0.3);
			can_dash_tween.Finished += OnDahshFinish;
			gravityMultiplier = 0;
		}
		Velocity = velocity;
	}
	void ApplyGravity(double delta)
	{
		var velocity = Velocity;
		velocity.Y += (float)(gravity * delta);
		velocity.Y = fasterFall && velocity.Y < 0 ? velocity.Y / 2 : velocity.Y;
		velocity.Y = velocity.Y * gravityMultiplier;
		velocity.Y = Mathf.Min(velocity.Y, terminalVelocity);
		Velocity = velocity;
	}

	void OnDahshFinish()
	{
		var velocity = Velocity;
		velocity.X = Mathf.MoveToward(velocity.X, 0, 500);
		gravityMultiplier = 1;
		Velocity = velocity;
	}

	public void BlockMovement()
	{
		canMove = false;
		Velocity = Vector2.Zero;
		graphics.UpdateLegs(Vector2.Zero, IsOnFloor(), crouch);
	}

	public Camera2D GetCam()
	{
		return camera;
	}

	public List<Material> GetMaterials()
	{
		return new List<Material> { graphics.legs.Material, graphics.torso.Material };
	}

    protected override void TriggerDeath()
	{
		GetTree().ReloadCurrentScene();
	}
}
