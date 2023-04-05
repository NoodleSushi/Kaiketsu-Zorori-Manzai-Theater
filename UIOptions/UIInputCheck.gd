extends "UIOptionBase.gd"
tool

export(bool) var default_check := false setget set_default_check
var check := true setget set_check

onready var checkbox: CheckBox = $CheckBox

func set_default_check(value: bool) -> void:
	default_check = value
	if not is_ready:
		yield(self, "ready")
	set_check(default_check)

func set_check(value: bool) -> void:
	check = value
	if not is_ready:
		yield(self, "ready")
	checkbox.pressed = check

func _on_CheckBox_toggled(button_pressed: bool) -> void:
	check = button_pressed

func get_default_value():
	return default_check

func get_value():
	return check

func set_value(value):
	if is_value_valid(value):
		set_check(value)

func is_value_valid(value) -> bool:
	return value is bool
