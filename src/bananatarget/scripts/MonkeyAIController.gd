extends AIController3D

# CS Types
const BananaPool := preload("res://scripts/BananaPool.cs")
const Monkey := preload("res://scripts/Monkey.cs")
const GameManager := preload("res://scripts/GameManager.cs")


@onready var monkey: Monkey = get_parent().get_node("Monkey")
@onready var game_manager: GameManager = get_parent().get_node("GameManager")
@onready var banana_pool: BananaPool = monkey.get_node("BananaPool")

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
	
	var banana_pos: Vector3
	var banana_lin_vel: Vector3
	var banana_ang_vel: Vector3
	
	var is_banana_thrown = not banana_pool.IsActivePoolEmpty()
	
	if is_banana_thrown:
		var current_banana = banana_pool.GetLatestBanana()
		banana_pos = to_local(current_banana.global_position)
		banana_lin_vel = to_local(current_banana.linear_velocity)
		banana_ang_vel = to_local(current_banana.angular_velocity)
	else:
		banana_pos = Vector3.ZERO
		banana_lin_vel = Vector3.ZERO
		banana_ang_vel = Vector3.ZERO
		

	var obs = [
		monkey_pos.x, monkey_pos.y, monkey_pos.z,
		monkey_vel.x, monkey_vel.y, monkey_vel.z,
		banana_pos.x, banana_pos.y, banana_pos.z,
		banana_lin_vel.x, banana_lin_vel.y, banana_lin_vel.z,
		banana_ang_vel.x, banana_ang_vel.y, banana_ang_vel.z,
		is_banana_thrown, cooldown, monkey_yaw
	]
	

	if is_banana_thrown:
		print(obs)
	
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
	if monkey.IsThrowing:
		return
		
	move_action = int(action["move_action"])


func _on_timer_timeout() -> void:
	done = true
