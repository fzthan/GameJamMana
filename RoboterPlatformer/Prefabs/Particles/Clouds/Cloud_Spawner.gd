extends CSGBox3D

@export var clouds_to_spawn : int = 10
@export var cloud : PackedScene

var random = RandomNumberGenerator.new()

# Called when the node enters the scene tree for the first time.
func _ready():
	spawn_clouds()

func spawn_clouds():
	while clouds_to_spawn >= 0:
		clouds_to_spawn -= 1
		
		var x : float = random.randf_range(size.x / 2, -size.x /2)
		var y : float = random.randf_range(size.y / 2, -size.y /2)
		var z : float = random.randf_range(size.z / 2, -size.z /2)
		
		var spawn_pos = Vector3(x, y, z)
		
		var prefab_cloud := cloud.instantiate()
		add_child(prefab_cloud)
		prefab_cloud.global_position = self.global_position + spawn_pos
