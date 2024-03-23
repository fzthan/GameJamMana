extends MeshInstance3D

var time_Elapsed = 0

var bullet = load("res://Bullet/Bullet.tscn")
var instance

@onready var turret = $RayCast3D

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	time_Elapsed += delta
	
	if (time_Elapsed >= 2.00):
		time_Elapsed = 0
		print("asdf")
		
		instance = bullet.instantiate()
		instance.position = turret.global_position
		instance.transform.basis = turret.global_transform.basis
		get_parent().add_child(instance)
