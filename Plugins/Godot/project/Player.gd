class_name  Player
extends Node2D

const BocchiTrackerLocation = preload("res://addons/bocchi_tracker_godot/data/bocchi_tracker_location.gd")

# インスペクタで設定可能な変数
var amplitude: float = 100
var frequency: float = 2
var speed: float = 100
var time_passed: float = 0

var initial_position: Vector2

func _ready():
	initial_position = position

func _process(delta: float):
	time_passed += delta
	
	var offset = amplitude * sin(frequency * time_passed)
	
	position.x = initial_position.x + offset

	position.x += speed * delta
	
	if position.x > get_viewport_rect().size.x:
		position.x = -get_viewport_rect().size.x

	if position.x < -get_viewport_rect().size.x:
		position.x = get_viewport_rect().size.x
		
func create_bocchi_tracker_ticket_infomation():
	return BocchiTrackerLocation.new("TEST Stage", Vector3(position.x, position.y, 0.0))
