extends Node3D

@export var global_seed: int = 42
var children: Array[Node]

func _ready() -> void:
	children = get_children()
	for idx in range(len(children)):
		if children[idx].is_in_group("game"):
			children[idx].level_seed = global_seed + idx
