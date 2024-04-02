extends Control

var master_bus_index: int;

func _ready():
  master_bus_index = AudioServer.get_bus_index("Master");

func onNewGame():
  get_tree().change_scene_to_file("res://Level/Large.tscn")


func onExit():
  get_tree().quit()


func onValueChanged(value):
  AudioServer.set_bus_volume_db(master_bus_index, linear_to_db(value))
