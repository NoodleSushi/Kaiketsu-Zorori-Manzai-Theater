extends Control
tool

export(String) var option_name := "option"
export(String) var option_label := "Option" setget set_option_label

onready var _label: Label = $OptionLabel

onready var is_ready := true

func set_option_label(value: String) -> void:
	option_label = value
	if not is_ready:
		yield(self, "ready")
	_label.text = option_label

func get_default_value():
	return null

func get_value():
	return null

func set_value(value):
	pass

func is_value_valid(value) -> bool:
	return true

