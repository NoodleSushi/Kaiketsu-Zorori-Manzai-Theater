extends "UIOptionBase.gd"
tool

enum ValueMode {INTEGER, STRING}

export(ValueMode) var value_mode := ValueMode.INTEGER
export(Array, String) var options_typed := [] setget set_options_typed
export(Array, String) var options_display := [] setget set_options_display
export(int) var default_option := 0 setget set_default_option
var option := 0 setget set_option

onready var option_button: OptionButton = $OptionButton

func set_options_typed(value: Array) -> void:
	options_typed = value

func set_options_display(value: Array) -> void:
	options_display = value
	if not is_ready:
		yield(self, "ready")
	option_button.clear()
	for option_display in options_display:
		option_button.add_item(option_display)
	set_option(option)

func set_default_option(value: int) -> void:
	default_option = value
	set_option(default_option)
	

func set_option(value: int) -> void:
	option = clamp(value, 0, options_display.size() - 1)
	if not is_ready:
		yield(self, "ready")
	if option_button.get_item_count() > 0:
		option_button.select(option)

func _on_OptionButton_item_selected(index: int) -> void:
	option = index

func get_default_value():
	if value_mode == ValueMode.STRING:
		return options_typed[default_option]
	return default_option

func get_value():
	if value_mode == ValueMode.STRING:
		return options_typed[option]
	return option

func set_value(value):
	if not is_value_valid(value):
		return
	if value_mode == ValueMode.STRING:
		set_option(options_typed.find(value))
	elif value_mode == ValueMode.INTEGER:
		set_option(value)

func is_value_valid(value) -> bool:
	var val = value
	if value_mode == ValueMode.STRING and val is String:
		val = options_typed.find(value)
	if val is int:
		return val >= 0 and val < options_display.size()
	return false
