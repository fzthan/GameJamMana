extends Node3D

var timer: Timer
var animation_tree : AnimationTree
# Called when the node enters the scene tree for the first time.
func _ready():
	timer = get_node("IdleTimer")
	timer.timeout.connect(_on_idle_timer_timeout)
	animation_tree = get_node("AnimationTree")
	animation_tree.animation_finished.connect(_on_anim_finish)

func _on_idle_timer_timeout():
	animation_tree["parameters/conditions/idleSecond"] = true
	
func _on_anim_finish(anim_name: StringName):
	if(anim_name == "MenuPlayerAnim/Idle2_menu"):
		animation_tree["parameters/conditions/idleSecond"] = false
		timer.start()
