class_name BocchiTrackerPlayerUpdater

extends Node

var disntance_threshould = 100.0
var prev_position : Vector3

var tracker_node = null

func _ready():
	tracker_node = get_parent()
	assert(tracker_node != null, "require parent node")
	assert(tracker_node.has_method("create_bocchi_tracker_ticket_infomation"), "need to implement \"fn create_bocchi_tracker_ticket_infomation()\"")

func _process(delta):
	var position = current_bocchi_pos()
	var distance = position.distance_to(prev_position)

	if distance >= disntance_threshould:
		var info = tracker_node.create_bocchi_tracker_ticket_infomation()
		get_tree().call_group("BocchiReciveTicketInfo", "on_bocchi_recive_ticket_info", info)
		prev_position = position

func current_bocchi_pos():
	var position = tracker_node.position
	
	if position is Vector3:
		return position
	elif position is Vector2:
		return Vector3(position.x, position.y, 0.0)
	else:
		assert(false, "ops!")
	
