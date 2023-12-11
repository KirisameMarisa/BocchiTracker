@tool
extends EditorPlugin


func _enter_tree():
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

func _exit_tree():
	# Clean-up of the plugin goes here.
	pass

func _disable_plugin() -> void:
	push_warning("BocchiTracker Addon got disabled. PLEASE RESTART THE EDITOR!")

