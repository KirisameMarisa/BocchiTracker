@tool
extends EditorPlugin

func _enter_tree():
	call_deferred("deferred_initialize")
	
func _exit_tree():
	call_deferred("deferred_finalize")

func deferred_initialize():
	ProjectSettings.set_setting("BocchiTracker/Setting/enabled",false)
	ProjectSettings.set_as_basic("BocchiTracker/Setting/enabled",true)
	ProjectSettings.set_initial_value("BocchiTracker/Setting/enabled",false)
	ProjectSettings.set_restart_if_changed("BocchiTracker/Setting/enabled",true)

	ProjectSettings.set_setting("BocchiTracker/Setting/ServerAddress", "")
	ProjectSettings.set_as_basic("BocchiTracker/Setting/ServerAddress",true)
	ProjectSettings.set_initial_value("BocchiTracker/Setting/ServerAddress", "")

	ProjectSettings.set_setting("BocchiTracker/Setting/ServerPort", 8888)
	ProjectSettings.set_as_basic("BocchiTracker/Setting/ServerPort",true)
	ProjectSettings.set_initial_value("BocchiTracker/Setting/ServerPort", 8888)
	add_autoload_singleton("BocchiSystemSingleton", "res://addons/bocchi_tracker_godot/bocchi_tracker_system.gd")
	
func deferred_finalize():
	remove_autoload_singleton("BocchiSystemSingleton")
