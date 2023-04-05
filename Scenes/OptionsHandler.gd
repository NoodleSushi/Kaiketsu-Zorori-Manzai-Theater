extends Control


enum OpenMode {LOAD, SAVE, RESTORE_DEFAULT}

const UIOptionBase = preload("res://UIOptions/UIOptionBase.gd")
const SECTION := "settings"
const CFG_PATH := "user://settings.cfg"


onready var options_list: Node = $OptionsSeparator/OptionsList
onready var apply_settings_button: Button = $OptionsSeparator/ApplySettings
onready var restore_default_button: Button = $OptionsSeparator/RestoreDefault
onready var back_button: Button = $BackButton

func _ready() -> void:
	open_cfg(OpenMode.LOAD, true)
	apply_settings_button.connect("pressed", self, "_on_apply_settings_button_pressed")
	back_button.connect("pressed", self, "_on_back_button_pressed")
	restore_default_button.connect("pressed", self, "_on_restore_default_button_pressed")

func _on_apply_settings_button_pressed() -> void:
	open_cfg(OpenMode.SAVE, true)

func _on_back_button_pressed() -> void:
	open_cfg(OpenMode.LOAD)

func _on_restore_default_button_pressed() -> void:
	open_cfg(OpenMode.RESTORE_DEFAULT, true)

func read_cfg(cfg: ConfigFile) -> void:
	var val
	val = cfg.get_value(SECTION, "lang", OS.get_locale_language())
	if val is String:
		TranslationServer.set_locale(val)
	var is_ja: bool = (TranslationServer.get_locale() == "ja")
	find_ui_option("captions").visible = is_ja
	val = cfg.get_value(SECTION, "fullscreen", false)
	if val is bool:
		OS.window_fullscreen = val

func find_ui_option(option_name: String) -> UIOptionBase:
	for child in options_list.get_children():
		if not (child is UIOptionBase):
			continue
		var ui_option: UIOptionBase = child
		if ui_option.option_name == option_name:
			return child
	return null

func open_cfg(open_mode: int, update := false) -> void:
	var cfg := ConfigFile.new()
	cfg.load(CFG_PATH)
	for child in options_list.get_children():
		if not (child is UIOptionBase):
			continue
		var ui_option: UIOptionBase = child
		if open_mode == OpenMode.LOAD:
			if cfg.has_section_key(SECTION, ui_option.option_name) and \
				ui_option.is_value_valid(cfg.get_value(SECTION, ui_option.option_name)):
				ui_option.set_value(cfg.get_value(SECTION, ui_option.option_name))
			else:
				cfg.set_value(SECTION, ui_option.option_name, ui_option.get_default_value())
		elif open_mode == OpenMode.SAVE:
			cfg.set_value(SECTION, ui_option.option_name, ui_option.get_value())
			ui_option.set_value(cfg.get_value(SECTION, ui_option.option_name))
		elif open_mode == OpenMode.RESTORE_DEFAULT:
			ui_option.set_value(ui_option.get_default_value())
			cfg.set_value(SECTION, ui_option.option_name, ui_option.get_default_value())
	if update:
		read_cfg(cfg)
	cfg.save(CFG_PATH)
