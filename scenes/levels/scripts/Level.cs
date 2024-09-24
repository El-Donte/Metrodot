using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

public partial class Level : Node2D
{
    PackedScene explosityScene = GD.Load("res://scenes/projectiles/scenes/explosion.tscn") as PackedScene;
	PackedScene bulletScene = GD.Load("res://scenes/projectiles/scenes/bullet.tscn") as PackedScene;

	[Export] Vector4I cameraLimit;
	Camera2D camera;
    public override void _Ready()
	{
		//camera
		camera = ((Player)GetTree().GetFirstNodeInGroup("Player")).GetCam();

		camera.LimitLeft = cameraLimit.X;
		camera.LimitRight = cameraLimit.Z;
		camera.LimitTop = cameraLimit.Y;
		camera.LimitBottom = cameraLimit.W;

		for (int i = 0; i < GetNode<Node>("Main/Entities").GetChildCount(); i++)
		{
			Entity entity = GetNode<Node>("Main/Entities").GetChild(i) as Entity;
            if (entity.HasSignal("Shoot"))
            {
				if (entity is Player)
				{
					Player player = entity as Player;
					player.Shoot += CreateBullet;
				}
				if (entity is Soldier)
				{
					Soldier soldier = entity as Soldier;
					soldier.Shoot += CreateBullet;
				}
				if (entity is Monster)
				{
					Monster monster = entity as Monster;
					monster.Shoot += CreateBullet;
				}

			}

			if (entity.HasSignal("Detonate"))
			{
				if (entity is Drone)
				{
					Drone drone = entity as Drone;
					drone.Detonate += CreateExplosion;
				}
				if (entity is Monster)
				{
					Monster monster = entity as Monster;
					monster.Detonate += CreateExplosion;
				}
			}
			/*
			//if (Global.enemyData.ContainsKey(currentScene))
			//{
			//	//if (entity is Player)
			//	//{
			//	//	Player player = entity as Player;
			//	//	player.Setup(Global.enemyData[currentScene][i]);
			//	//}
			//	//if (entity is Soldier)
			//	//{
			//	//	Soldier soldier = entity as Soldier;
			//	//	soldier.Setup(Global.enemyData[currentScene][i]);
			//	//}
			//	//if (entity is Monster)
			//	//{
			//	//	Monster monster = entity as Monster;
			//	//	monster.Setup(Global.enemyData[currentScene][i]);
			//	//}
			//	//if (entity is Drone)
			//	//{
			//	//	Drone drone = entity as Drone;
			//	//	drone.Setup(Global.enemyData[currentScene][i]);
			//	//}
			//	entity.Setup(Global.enemyData[currentScene][i]);
			//}
			*/
		}

		

    }

	void CreateBullet(Vector2 position,Vector2 direction,int bulletType)
	{
        var bullet = bulletScene.Instantiate<Bullet>();
        GetNode<Node2D>(new NodePath("Main/Projectiles")).AddChild(bullet);
		bullet.Setup(position,direction,bulletType);
		if (bulletType == 2)
		{
			bullet.Detonate += CreateExplosion;
		}
		
    }
	void CreateExplosion(Vector2 position)
	{
		var explosion = explosityScene.Instantiate<Explosion>();
        GetNode<Node2D>(new NodePath("Main/Projectiles")).AddChild(explosion);
		explosion.Position = position;
    }

  //  public override void _ExitTree()
  //  {
  //      List<Entity> currentEnemyData = new List<Entity>();
		//foreach(Entity entity in GetNode("Main/Entities").GetChildren())
		//{
		//	currentEnemyData.Add(entity);
  //          GetNode("Main/Entities").RemoveChild(entity);
  //      }
		//Global.enemyData[GetTree().CurrentScene.Name] =  currentEnemyData;
  //  }

}
