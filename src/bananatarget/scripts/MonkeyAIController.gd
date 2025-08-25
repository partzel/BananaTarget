extends AIController3D

@onready var monkey: Node3D = get_parent().get_node("Monkey")
@onready var game_manager: Node = get_parent().get_node("GameManager")
var move_action : int = 0
var reward_step: float = 0.0

func _ready() -> void:
	assert(game_manager != null, "Could not find the game manager!")
	init(monkey)

func _process(delta: float) -> void:
	reward_step = game_manager.Reward
	game_manager.Reward = 0.0

func _physics_process(delta: float) -> void:
	if needs_reset and game_manager:
		reset()
		game_manager.StartRound()
		return
		
		
	if heuristic != "human":
		match move_action:
			0:
				monkey.DoNothing(delta)
			1:
				monkey.Throw()
			2:
				monkey.RotateRight(delta)
			3:
				monkey.RotateLeft(delta)
			4:
				monkey.MoveForward(delta);
			5:
				monkey.MoveBackward(delta);
		
		monkey.move_and_slide();


func get_obs() -> Dictionary:
	var monkey_pos = to_local(monkey.global_position)
	var monkey_vel = to_local(monkey.velocity)
	var monkey_yaw = monkey.rotation.y
	var cooldown = monkey.CooldownRemaining / monkey.ThrowCooldown
	var is_throwing = monkey.IsThrowing

	var obs = [
		monkey_pos.x, monkey_pos.y, monkey_pos.z,
		monkey_vel.x, monkey_vel.y, monkey_vel.z,
		is_throwing, cooldown, monkey_yaw
	]
	return {"obs":obs}


func get_reward() -> float:
	return reward_step

func zero_reward() -> void:
	reward_step = 0.0

func get_action_space() -> Dictionary:
	return {
		"move_action" : {
			"size": 6,
			"action_type": "discrete"
		},
	}


func set_action(action) -> void:
	move_action = int(action["move_action"])


func _on_timer_timeout() -> void:
	done = true
