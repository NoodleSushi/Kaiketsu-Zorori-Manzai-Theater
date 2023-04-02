extends KinematicBody


export(float) var weight: float = 4
export(float) var speed: float = 5
export(float) var jump_velocity: float = 10
export(float) var camera_rotation: float = 0.003

var velocity: Vector3 = Vector3()
var move_velocity: Vector3 = Vector3()
var accelerator: float = 0
var camera: Camera
var camera_spatial: Spatial
var animator: AnimationPlayer
var tween: Tween = Tween.new()

var mouse_pressed: bool = true

func _ready() -> void:
	if OS.get_name() == "Windows":
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	
	add_child(tween)
	camera_spatial = $CameraSpatial
	camera = $CameraSpatial/Camera
	animator = $"../CanvasLayer/AnimationPlayer"

func _physics_process(delta: float) -> void:
	velocity.y -= weight*delta
	
	var mover: Vector2 = Vector2(
		Input.get_action_strength("ui_right")-Input.get_action_strength("ui_left"),
		Input.get_action_strength("ui_up")-Input.get_action_strength("ui_down")
	)
	
	if mover.length() > 0:
		move_velocity = Vector3()
		move_velocity += transform.basis.x * mover.x
		move_velocity -= transform.basis.z * mover.y
	
	accelerator = lerp(accelerator, sign(mover.length()), 0.25 if sign(mover.length()) == 0 else 0.1)
	
	move_velocity = move_velocity.normalized()*speed*accelerator
	velocity = Vector3(move_velocity.x, velocity.y, move_velocity.z)
	
	var was_on_floor := is_on_floor()
	
	var prev_yv: float = velocity.y
	
	if Input.is_action_just_pressed("ui_accept") && was_on_floor:
		velocity.y = jump_velocity
	
	velocity = move_and_slide(velocity, Vector3.UP, true)
	
	if !was_on_floor && is_on_floor():
		tween.interpolate_property(camera, "translation:y", -0.1, 0, 0.4, Tween.TRANS_BACK,Tween.EASE_OUT)
		tween.start()

func _input(event: InputEvent) -> void:
	if event is InputEventMouseButton && event.button_index == BUTTON_LEFT:
		mouse_pressed = event.pressed
	
	if event is InputEventMouseMotion && (OS.get_name() == "Windows" || (OS.get_name() == "HTML5" && mouse_pressed)):
		var offset: Vector2 = event.relative * camera_rotation
		rotation_degrees.y -= offset.x
		camera.rotation_degrees.x = clamp(camera.rotation_degrees.x - offset.y, -90, 90)


func _on_DoorArea_body_shape_entered(body_id: int, body: Node, body_shape: int, local_shape: int) -> void:
	if body == self:
		animator.play("Exit")
