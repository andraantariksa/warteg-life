extends Button

func _pressed():
	assert(get_tree().change_scene("res://LevelSelectorScene.tscn") == OK)
