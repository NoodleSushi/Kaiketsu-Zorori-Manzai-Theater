extends Button

export var action: String

func _gui_input(event):
	if event is InputEventMouseButton and event.button_index == BUTTON_LEFT and event.is_pressed():
		# Perform action when left mouse button is pressed on button
		Input.action_press(action)
