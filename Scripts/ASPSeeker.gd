extends AnimationNodeOneShot
class_name ASPSeeker
tool
const CAPTION = "ASPSeeker"

#var time_scale : float = 0
#var start_time : float = 0
#var playing : bool = false
#var audio_seek : float = 0
var timer : float = 0

func process(time: float, seek: bool) -> void:
	if seek:
		return
	blend_input(0, time, false, 1.0)
	if get_parameter("playing"):
		timer += time
		print(blend_input(1, timer, true, 1.0, 3))
	else:
		timer = 0

func get_caption() -> String:
	return CAPTION

func get_parameter_list() -> Array:
	return [
		{
			name = "time_scale",
			type = TYPE_REAL
		},
		{
			name = "start_time",
			type = TYPE_REAL
		},
		{
			name = "playing",
			type = TYPE_BOOL
		},
		{
			name = "audio_player",
			type = TYPE_OBJECT
		}
	]
