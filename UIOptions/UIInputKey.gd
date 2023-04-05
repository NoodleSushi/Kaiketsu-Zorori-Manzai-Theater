extends "UIOptionBase.gd"
tool

class Glob:
	var is_detecting := false

signal input_detected

export(String) var action := ""
export(String) var default_key := "" setget set_default_key

var is_detecting := false
var glob_ref: Glob = null

onready var key_input: Button = $KeyInput

func set_default_key(value: String) -> void:
	default_key = value
	if not Engine.editor_hint:
		return
	if not is_ready:
		yield(self, "ready")
	key_input.text = default_key if is_valid_key(default_key) else "INVALID KEY"

static func is_valid_key(key: String) -> bool:
	if key.length() == 1: return true
	if key in [
			"Up", "Down", "Left", "Right", 
			"Space", "Enter", 
			"Comma", "Period", 
			"Slash", "BackSlash", 
			"Minus", "Equal", 
			"Semicolon", "Apostrophe",
			"BracketLeft", "BracketRight"
		]: return true
	return false

func _ready() -> void:
	if Engine.editor_hint:
		return
	InputHelper.set_action_key(action, default_key)
	key_input.text = InputHelper.get_action_key(action)
	key_input.connect("pressed", self, "_on_KeyInput_pressed")
	InputHelper.connect("action_key_changed", self, "_on_InputHelper_action_key_changed")
	for child in get_parent().get_children():
		if child is get_script() and child.glob_ref != null:
			glob_ref = child.glob_ref
			break
	if glob_ref == null:
		glob_ref = Glob.new()
	
func _unhandled_input(event: InputEvent) -> void:
	if not is_detecting:
		return
	if event is InputEventKey and event.is_pressed():
		accept_event()
		InputHelper.set_action_key(action, event.as_text())
		emit_signal("input_detected")

func _on_KeyInput_pressed() -> void:
	if is_detecting or glob_ref.is_detecting:
		return
	is_detecting = true
	glob_ref.is_detecting = true
	key_input.disabled = true
	key_input.text = "SET_CTRL_KEYDET"
	yield(self, "input_detected")
	key_input.disabled = false
	key_input.pressed = false
	is_detecting = false
	glob_ref.is_detecting = false

func _on_InputHelper_action_key_changed(action_name, key) -> void:
	if action_name == action:
		key_input.text = key
		if is_detecting:
			emit_signal("input_detected")

func get_default_value():
	return default_key

func get_value():
	return InputHelper.get_action_key(action)

func set_value(value):
	if is_value_valid(value):
		InputHelper.set_action_key(action, value)

func is_value_valid(value) -> bool:
	return is_valid_key(value)
