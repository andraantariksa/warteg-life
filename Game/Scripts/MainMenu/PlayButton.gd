extends Button

func _pressed():
	assert(get_tree().change_scene("res://Scene/LevelSelectorScene.tscn") == OK)
