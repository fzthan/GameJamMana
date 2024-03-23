extends Node3D

const SPEED = 40.0

@onready var mesh = $MeshInstance3D
@onready var ray = $RayCast3D

@onready var yellow = $Yellow
@onready var red = $Red


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	position += transform.basis * Vector3(0, 0, -SPEED) * delta
	if ray.is_colliding():
		mesh.visible = false
		yellow.emitting = true
		red.emitting = true
		print("collided")
		print(ray.get_collider())
		
		if ray.get_collider().is_in_group("player"):
			print("damage")
			ray.get_collider().call("TakeDamage", 5)
			ray.enabled = false
		
		await get_tree().create_timer(3.0).timeout
		$".".queue_free()

func _on_timer_timeout():
	$".".queue_free()
