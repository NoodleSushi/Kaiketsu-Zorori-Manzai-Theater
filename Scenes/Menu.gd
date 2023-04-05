extends Node

onready var AudioBGM: AudioStreamPlayer = $AudioStreamPlayer
onready var AudioBoop: AudioStreamPlayer = $AudioBoop
onready var main_gui: Control = $CanvasLayer/GUI
onready var credits_gui: Control = $CanvasLayer/Credits
onready var options_gui: Control = $CanvasLayer/Options
onready var bg_dim: Control = $CanvasLayer/BGDim
onready var camera: Node2D = $Background/CameraParent
onready var credits_label: RichTextLabel = $CanvasLayer/Credits/CenterContainer/RichTextLabel

const DEF_EASE := Tween.TRANS_EXPO
const DEF_TYPE := Tween.EASE_OUT
const DEF_TIME := 0.6

var tween := Tween.new()
var music_tween := Tween.new()

var scene_switch: String = ""
var pressed := false

func _ready() -> void:
	add_child(tween)
	add_child(music_tween)
	if OS.get_name() == "Windows":
		Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
		
	main_gui.visible = true
	
	credits_gui.visible = true
	credits_gui.anchor_left = -1
	credits_gui.anchor_right = 0
	
	options_gui.visible = true
	options_gui.anchor_left = 1
	options_gui.anchor_right = 2
	
	bg_dim.self_modulate = Color(1, 1, 1, 0)

func _on_SnootButton_pressed() -> void:
	if pressed: return
	AudioBoop.play()
	SceneTransition.transition_to_scene("res://Scenes/EasterEgg.tscn")
	fade_music()
	pressed = true

func _on_PlayButton_pressed() -> void:
	if pressed: return
	SceneTransition.transition_to_scene("res://Scenes/Game.tscn")
	fade_music()
	pressed = true

func fade_music() -> void:
	music_tween.interpolate_property(AudioBGM, "volume_db", AudioBGM.volume_db, -80, 1,Tween.TRANS_SINE,Tween.EASE_IN)
	music_tween.start()


func _on_RichTextLabel_meta_clicked(meta) -> void:
	OS.shell_open(str(meta))


func _on_CreditsButton_pressed() -> void:
	credits_label.bbcode_text = tr("TXT_CREDITS")
	tween.stop_all()
	tween.interpolate_property(credits_gui, "anchor_left", credits_gui.anchor_left, 0, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(credits_gui, "anchor_right", credits_gui.anchor_right, 1, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(main_gui, "anchor_left", main_gui.anchor_left, 1, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(main_gui, "anchor_right", main_gui.anchor_right, 2, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(options_gui, "anchor_left", options_gui.anchor_left, 2, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(options_gui, "anchor_right", options_gui.anchor_right, 1, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(camera, "position:y", camera.position.y, -500, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(bg_dim, "self_modulate", bg_dim.self_modulate, Color.white, DEF_TIME * .5, DEF_EASE, DEF_TYPE)
	tween.start()


func _on_BackButton_pressed() -> void:
	tween.stop_all()
	tween.interpolate_property(credits_gui, "anchor_left", credits_gui.anchor_left, -1, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(credits_gui, "anchor_right", credits_gui.anchor_right, 0, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(main_gui, "anchor_left", main_gui.anchor_left, 0, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(main_gui, "anchor_right", main_gui.anchor_right, 1, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(options_gui, "anchor_left", options_gui.anchor_left, 1, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(options_gui, "anchor_right", options_gui.anchor_right, 2, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(camera, "position:y", camera.position.y, 0, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(bg_dim, "self_modulate", bg_dim.self_modulate, Color(1, 1, 1, 0), DEF_TIME * .5, DEF_EASE, DEF_TYPE)
	tween.start()


func _on_OptionsButton_pressed() -> void:
	tween.stop_all()
	tween.interpolate_property(credits_gui, "anchor_left", credits_gui.anchor_left, -2, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(credits_gui, "anchor_right", credits_gui.anchor_right, -1, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(main_gui, "anchor_left", main_gui.anchor_left, -1, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(main_gui, "anchor_right", main_gui.anchor_right, 0, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(options_gui, "anchor_left", options_gui.anchor_left, 0, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(options_gui, "anchor_right", options_gui.anchor_right, 1, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(camera, "position:y", camera.position.y, -500, DEF_TIME, DEF_EASE, DEF_TYPE)
	tween.interpolate_property(bg_dim, "self_modulate", bg_dim.self_modulate, Color.white, DEF_TIME * .5, DEF_EASE, DEF_TYPE)
	tween.start()
