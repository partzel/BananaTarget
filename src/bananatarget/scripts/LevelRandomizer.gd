extends Node3D

@export var level_seed: int = 42

func _ready() -> void:
	seed(level_seed)
	randomize()
