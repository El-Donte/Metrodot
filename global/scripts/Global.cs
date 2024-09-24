using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Global : Node
{
	static Resource akShoot = GD.Load("res://audio/ak_shoot.wav");
	static Resource rocketShoot = GD.Load("res://audio/shotgun_shoot.wav");
	static Resource shotgunShoot = GD.Load("res://audio/rocket_shoot.wav");
	static Resource monsterShoot = GD.Load("res://audio/monster_shoot.wav");
    public enum Guns
	{
		AK = 0, SHOTGUN = 1, ROCKET = 2, Monster = 3
	}

	static System.Collections.Generic.Dictionary<string, int> emeny_paramets = new System.Collections.Generic.Dictionary<string, int>()
	{
		{"speedDrone", 110 },
		{"speedSoldier", 50 },
		{"healthSoldier", 50 },
		{"healthDrone", 50 },
		{"healthMonster", 500 },
        {"damageDrone", 10 }
    };

	public static System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, int>> enemy = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, int>>()
	{
		{"drone",emeny_paramets},
		{"soldier",emeny_paramets},
		{"monster",emeny_paramets}
	};


	public static Godot.Collections.Dictionary<int, Resource> bulletSound = new Godot.Collections.Dictionary<int, Resource>()
	{
		{0,akShoot},
		{1,shotgunShoot},
		{2,rocketShoot},
		{3,monsterShoot}
	};

    //public static Dictionary<string, List<Entity>> enemyData = new Dictionary<string, List<Entity>>();
}
