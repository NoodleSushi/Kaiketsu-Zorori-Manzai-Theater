extends Camera2D

func _process(delta: float) -> void:
	var viewport_rect : Rect2 = get_viewport_rect()
	var mouse_pos : Vector2 = get_viewport().get_mouse_position()
	position = 8 * (mouse_pos - viewport_rect.size / 2) / viewport_rect.size.y
