@tool
extends Area2D

@export_enum('station', 'sky', 'station2') var level = 'station'
var levels = {}

func _ready():
	if not Engine.is_editor_hint():
		levels['station'] = "res://scenes/levels/scenes/station.tscn"
		levels['sky'] = "res://scenes/levels/scenes/sky.tscn"
		levels['station2'] = "res://scenes/levels/scenes/station2.tscn"

func _on_body_entered(_body):
	TransitionLayer.ChangeScence(levels[level])
