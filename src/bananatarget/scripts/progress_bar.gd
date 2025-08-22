extends TextureProgressBar

@export var target: Node3D
@export var offset_3d: Vector3


func _process(_delta):
	var pos_3d := target.global_position + offset_3d
	var cam := get_viewport().get_camera_3d()
	var pos_2d := cam.unproject_position(pos_3d)
	global_position = pos_2d
