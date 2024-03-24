extends Node3D

@onready var sparks = $Sparks
@onready var flash = $Flash
@onready var fire = $Fire
@onready var smoke = $Smoke

var already_exploding = false

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	var player = get_parent().get_node("Player")
	
	if (player.get("Health") <= 0 || player.global_transform.origin.y < -9) && !already_exploding:
		global_transform.origin = player.global_transform.origin
		sparks.emitting = true
		flash.emitting = true
		fire.emitting = true
		smoke.emitting = true
		
		await get_tree().create_timer(2.0).timeout
		already_exploding = false
		
