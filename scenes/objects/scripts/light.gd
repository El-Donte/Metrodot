@tool
extends Node2D

@export_enum('0', '1', '2', '3', '4', '5','6') var type = '0':
	set(value):
		if get_child_count() > 0 and value != null:
			type = value
			for child in $Options.get_children():
				child.hide()
			$Options.get_child(int(type)).show()
@export_color_no_alpha var color = Color(1.0,1.0,1.0,1.0):
	set(value):
		if get_child_count() > 0 and value != null:
			color = value
			$Options.get_child(int(type)).get_child(1).color = value
@export_range(0,10) var strength := 1.0:
	set(value):
		if get_child_count() > 0 and value != null:
			strength = value
			$Options.get_child(int(type)).get_child(1).energy = value
@export_range(0,20) var radius := 1.0:
	set(value):
		if get_child_count() > 0 and value != null:
			radius = value
			$MainLight.texture_scale = value
@export_range(0,200) var flicker := 100.0

var noise = FastNoiseLite.new()
var value := 0.0
var active_light: PointLight2D

func _ready():
	for child in $Options.get_children():
		if child.visible:
			active_light = child.get_child(1)

func _process(delta):
	value += flicker * delta
	if active_light:
		active_light.energy = (noise.get_noise_1d(value) + 1 / 4.0) + 0.5
	
