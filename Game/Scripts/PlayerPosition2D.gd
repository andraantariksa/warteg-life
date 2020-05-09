extends Position2D

enum State {
	Idle,
	Follow
}

const SPEED = 200
const MASS = 10.0
const ARRIVE_DISTANCE = 10.0

var _state : int = State.Idle
var path : Array = []

var target_point_world : Vector2 = Vector2()
var target_position : Vector2 = Vector2()
var velocity : Vector2

func _ready():
	_change_state(State.Idle)

func _process(_delta : float):
	if not _state == State.Follow:
		return
	var arrived_to_next_point = move_to(target_point_world)
	if arrived_to_next_point:
		path.remove(0)
		if len(path) == 0:
			_change_state(State.Idle)
			return
		target_point_world = path[0]

func _input(event : InputEvent):
	if event is InputEventMouseButton:
		if Input.is_key_pressed(KEY_SHIFT):
			global_position = get_global_mouse_position()
		else:
			target_position = get_global_mouse_position()
		_change_state(State.Follow)

func _change_state(state_new : int):
	if state_new == State.Follow:
		path = get_parent().get_node("TileMap").get_tile_path(position, target_position)
		if len(path) <= 1:
			_state = State.Idle
			return
		target_point_world = path[1]
	_state = state_new

func move_to(world_position : Vector2) -> bool:
	var desired_velocity = (world_position - position).normalized() * SPEED
	var steering = desired_velocity - velocity
	velocity += steering / MASS
	position += velocity * get_process_delta_time()
	#rotation = velocity.angle()
	return position.distance_to(world_position) < ARRIVE_DISTANCE
