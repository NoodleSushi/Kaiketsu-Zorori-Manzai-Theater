extends Node


var scene_switch: String = ""

func _ready() -> void:
	if OS.get_name() == "Windows":
		Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)

func _on_SnootButton_pressed() -> void:
	SceneTransition.transition_to_scene("res://Scenes/EasterEgg.tscn")

func _on_PlayButton_pressed() -> void:
	SceneTransition.transition_to_scene("res://Scenes/Game.tscn")
