extends AIController3D

# CS Types
const ToucanPool := preload("res://scripts/ToucanPool.cs")
const BananaPool := preload("res://scripts/BananaPool.cs")
const Monkey := preload("res://scripts/Monkey.cs")
const GameManager := preload("res://scripts/GameManager.cs")


@onready var monkey: Monkey = get_parent().get_node("Monkey")
@onready var game_manager: GameManager = get_parent().get_node("GameManager")
@onready var banana_pool: BananaPool = monkey.get_node("BananaPool")
@onready var toucan_pool: ToucanPool = get_parent().get_node("ToucanPool")

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
				monkey.Throw()
			1:
				monkey.RotateRight(delta)
			2:
				monkey.RotateLeft(delta)
			3:
				monkey.MoveForward(delta);
			4:
				monkey.MoveBackward(delta);
		
		monkey.move_and_slide();


func get_obs() -> Dictionary:
	var monkey_pos = to_local(monkey.position)
	var monkey_vel = to_local(monkey.velocity)
	var monkey_yaw = monkey.rotation.y
	var toucan_pos = to_local(toucan_pool.GetToucanPosition())
	var cooldown = monkey.CooldownRemaining / monkey.ThrowCooldown
	
	var is_banana_thrown = not banana_pool.IsActivePoolEmpty()
	
	var obs = [
		monkey_pos.x, monkey_pos.y, monkey_pos.z,
		monkey_vel.x, monkey_vel.y, monkey_vel.z,
		toucan_pos.x, toucan_pos.y, toucan_pos.z,
		is_banana_thrown, cooldown, monkey_yaw
	]

	
	return {"obs":obs}


func get_reward() -> float:
	return reward_step

func zero_reward() -> void:
	reward_step = 0.0

func get_action_space() -> Dictionary:
	return {
		"move_action" : {
			"size": 5,
			"action_type": "discrete"
		},
	}


func set_action(action) -> void:
	if monkey.IsThrowing:
		return
		
	move_action = int(action["move_action"])


func _on_timer_timeout() -> void:
	done = true
