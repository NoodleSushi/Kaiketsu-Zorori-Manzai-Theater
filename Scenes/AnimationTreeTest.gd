extends AnimationTree


var timer = 0


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	set("parameters/audience_laughOneShot/active", true)


func _process(delta: float) -> void:
	if get("parameters/audience_laughOneShot/active") == false:
		set("parameters/audience_laughOneShot/active", true)
