extends "UIOptions.gd"
tool

func _ready() -> void:
	var new_default: int = options_typed.find(OS.get_locale_language())
	if new_default != -1:
		set_default_option(new_default)
