class_name BocchiTrackerSystem

extends Node

const BocchiTrackerTcpSocket = preload("res://addons/bocchi_tracker_godot/bocchi_tracker_tcp_socket.gd")
const BocchiTrackerIssue = preload("res://addons/bocchi_tracker_godot/data/bocchi_tracker_issue.gd")
const BocchiTrackerLocation = preload("res://addons/bocchi_tracker_godot/data/bocchi_tracker_location.gd")

var tcp_socket: BocchiTrackerTcpSocket

var jump_request := []

var is_sent_app_basic_info = false

var pending_process_request := []

func _ready():
	tcp_socket = BocchiTrackerTcpSocket.new()
	var ip_address = ProjectSettings.get_setting("BocchiTracker/Setting/ServerAddress", "127.0.0.1")
	var port  = ProjectSettings.get_setting("BocchiTracker/Setting/ServerPort", 8888)
	is_sent_app_basic_info = false
	
	if not is_connect():
		tcp_socket.initialize(ip_address, port)
	tcp_socket.receive_callback.connect(on_receive_data)
	tcp_socket.on_connect.connect(func (): is_sent_app_basic_info = false)

func _process(delta):
	if is_connect():
		if not is_sent_app_basic_info:
			process_send_app_basic_info()
		tcp_socket.update(delta)
		
		if pending_process_request.size() > 0:
			var out_request :Dictionary = pending_process_request.pop_front()
			var query_id = out_request["query_id"]

			match query_id:
				bocchi_api.QueryID_IssueesRequest:
					var issues = out_request["issues"]
					for issue in issues:
						cache_issue_info(issue)

				bocchi_api.QueryID_JumpRequest:
					var stage = out_request["stage"]
					var location = out_request["location"]
					jump_location(stage, location.x, location.y, location.z)

				bocchi_api.QueryID_ScreenshotRequest:
					process_send_screenshot()

func is_connect() -> bool:
	return tcp_socket.is_connect()

func on_receive_data(in_data: PackedByteArray):
	var data = bocchi_api.resolve_request_query(in_data)
	pending_process_request.append(data)

func process_send_app_basic_info():
	var pid 		= OS.get_process_id()
	var app_name 	= ProjectSettings.get_setting("application/config/name")
	var args:String = ""
	for arg in OS.get_cmdline_args():
		args += "-" + arg + " "
	var platform 	= OS.get_name()
	var log_file_path = ProjectSettings.globalize_path(ProjectSettings.get_setting("debug/file_logging/log_path"))
	
	var app_basic_info_packet = bocchi_api.create_application_basic_information(pid, app_name, args, platform, log_file_path)

	tcp_socket.add_send_data(app_basic_info_packet)
	is_sent_app_basic_info = true

func cache_issue_info(in_issue):
	# Implement the logic to cache issue info here
	pass

func jump_location(in_stage: String, in_x: float, in_y: float, in_z: float):
	if jump_request.size() == 0:
		jump_request.append(BocchiTrackerLocation.new(in_stage, Vector3(in_x, in_y, in_z)))

func process_send_screenshot():
	var screenshot = get_viewport().get_texture().get_image()
	var screenshot_data = PackedByteArray()

	for y in range(screenshot.get_height()):
		for x in range(screenshot.get_width()):
			var pixel = screenshot.get_pixel(x, y)
			screenshot_data.append(int(pixel.r * 255))
			screenshot_data.append(int(pixel.g * 255))
			screenshot_data.append(int(pixel.b * 255))
			screenshot_data.append(int(pixel.a * 255))

	var screenshot_data_packet = bocchi_api.create_screenshot_data(screenshot.get_width(), screenshot.get_height(), screenshot_data)
	tcp_socket.add_send_data(screenshot_data_packet)
	
func on_bocchi_recive_ticket_info(in_data):
	var packet = bocchi_api.create_player_position(in_data.location, in_data.stage)
	tcp_socket.add_send_data(packet)
