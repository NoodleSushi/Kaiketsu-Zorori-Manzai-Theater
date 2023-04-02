extends CanvasLayer


signal transition_closed

var _scene_path := ""
var is_reload = false
var is_transitioning = false

func _ready() -> void:
	$AnimationPlayer.play("Intro")

func reload_current_scene() -> void:
	if is_transitioning:
		return
	is_transitioning = true
	is_reload = true
	$AnimationPlayer.play("Exit")

func transition_to_scene(path: String) -> void:
	if is_transitioning:
		return
	is_transitioning = true
	_scene_path = path
	is_reload = false
	$AnimationPlayer.play("Exit")

func _on_SceneTransition_transition_closed() -> void:
	if is_reload:
		get_tree().reload_current_scene()
	else:
		get_tree().change_scene(_scene_path)
	is_transitioning = false
	$AnimationPlayer.play("Intro")
