extends TileMap

const HALF_TILE_SIZE : Vector2 = Vector2(32, 32)

var _astar : AStar2D = AStar2D.new()
var _rect_bound : Rect2
var _traversable_tiles : Array

func _ready():
	_traversable_tiles = get_used_cells()
	_rect_bound = get_used_rect()
	
	_add_traversable_tiles(_traversable_tiles)
	_connect_traversable_tiles(_traversable_tiles)

func _add_traversable_tiles(traversable_tiles : Array) -> void:
	for tile in traversable_tiles:
		var tile_id : int = _get_tile_id_for(tile)
		
		_astar.add_point(tile_id, Vector2(tile.x, tile.y))

func _connect_traversable_tiles(_traversable_tiles : Array) -> void:
	for tile in _traversable_tiles:
		var tile_id : int = _get_tile_id_for(tile)
		
		# Left top right bottom, no diagonal movement
		for x_or_y in range(2):
			for add in range(2):
				var target : Vector2 = tile
				
				if x_or_y == 0:
					target += Vector2(0, 1 - 2 * add)
				else:
					target += Vector2(1 - 2 * add, 0)
				
				var target_id : int = _get_tile_id_for(target)
				
				if not _astar.has_point(target_id):
					continue
				
				_astar.connect_points(tile_id, target_id, true)

func _get_tile_id_for(tile : Vector2) -> int:
	return int(tile.x + tile.y * _rect_bound.size.x)

func get_tile_path(from: Vector2, to: Vector2) -> Array:
	var tile_from : Vector2 = world_to_map(from)
	var tile_to : Vector2 = world_to_map(to)
	
	var tile_from_id : int = _get_tile_id_for(tile_from)
	var tile_to_id : int = _get_tile_id_for(tile_to)
	
	#if not astar.has_point(tile_from_id) or astar.has_point(tile_to_id):
	#	return null
	
	var path_map : Array = _astar.get_point_path(tile_from_id, tile_to_id)
	
	var path_world : Array = []
	for coord_map in path_map:
		var coord_world : Vector2 = map_to_world(Vector2(coord_map.x, coord_map.y)) + HALF_TILE_SIZE
		path_world.append(coord_world)
	
	return path_world
