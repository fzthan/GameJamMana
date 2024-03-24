extends Control


func onNewGame():
	get_tree().change_scene_to_file("res://Level/Large.tscn")


func onExit():
	get_tree().quit()


func onValueChanged(value):
	Wwise.set_rtpc_value_id(0, value, null)
	AudioServer.set_bus_volume_db(AudioServer.get_bus_index("Master"),value)
