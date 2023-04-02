extends Node


var scene_switch: String = ""

func _ready() -> void:
	if OS.get_name() == "Windows":
		Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)

func _on_SnootButton_pressed() -> void:
	scene_switch = "res://Scenes/EasterEgg.tscn"
	$CanvasLayer2/AnimationPlayer.play("Exit")

func _on_PlayButton_pressed() -> void:
	scene_switch = "res://Scenes/Game.tscn"
	$CanvasLayer2/AnimationPlayer.play("Exit")

func switch_to_scene() -> void:
	get_tree().change_scene(scene_switch)
