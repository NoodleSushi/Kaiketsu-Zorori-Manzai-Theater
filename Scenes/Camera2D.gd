extends Camera2D

func _process(delta: float) -> void:
	var viewport_rect : Rect2 = get_viewport_rect()
	var mouse_pos : Vector2 = get_viewport().get_mouse_position()
	position = position.linear_interpolate(Vector2(32, 16) * (mouse_pos - viewport_rect.size / 2) / viewport_rect.size.y, 1 - pow(.025, delta))
