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
	
	var direction = get_parent().get_node("Player").global_transform.origin
	print(direction)
	turret.look_at(direction, Vector3.UP)
	
	look_at(direction, Vector3.UP)
	rotation.x = 0
	rotation.z = 0
	
	
	if (time_Elapsed >= 2.00):
		time_Elapsed = 0
		
		instance = bullet.instantiate()
		instance.position = turret.global_position
		instance.transform.basis = turret.global_transform.basis
		get_parent().add_child(instance)
