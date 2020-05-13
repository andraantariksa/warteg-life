using Godot;
using System;
using System.Collections.Generic;

public class PathTile : TileMap
{
	private AStar2D astar;
	private Rect2 rectangleBound;
	private Godot.Collections.Array traversableTiles;
	private readonly Vector2 HALFTILESIZE = new Vector2(32, 32);

	public PathTile()
	{
		astar = new AStar2D();
	}

	public override void _Ready()
	{
		traversableTiles = GetUsedCells();
		rectangleBound = GetUsedRect();

		AddTraversableTiles(traversableTiles);
		ConnectTraversableTiles(traversableTiles);

		GetParent()
			.GetNode("Player/PlayerPosition")
			.Connect("PlayerMapFindPath", this, "SignalGetTilePath");
	}

	[Signal]
	delegate void PlayerSetPath(List<Vector2> path);

	public void SignalGetTilePath(Vector2 from, Vector2 to)
	{
		EmitSignal("PlayerSetPath", GetTilePath(from, to));
	}

	public List<Vector2> GetTilePath(Vector2 from, Vector2 to)
	{
		Vector2 tileFrom = WorldToMap(from);
		Vector2 tileTo = WorldToMap(to);

		int tileFromId = GetTileId(tileFrom);
		int tileToId = GetTileId(tileTo);

		if (!astar.HasPoint(tileFromId) || !astar.HasPoint(tileToId))
		{
			return null;
		}

		Vector2[] pathMap = astar.GetPointPath(tileFromId, tileToId);

		List<Vector2> pathWorld = new List<Vector2>();
		foreach (Vector2 coordMap in pathMap)
		{
			GD.Print("Coord ", coordMap);

			Vector2 coordWorld = MapToWorld(coordMap) + HALFTILESIZE;
			pathWorld.Add(coordWorld);
		}
		return pathWorld;
	}

	private int GetTileId(Vector2 tile)
	{
		return (int)(tile.x + tile.y * rectangleBound.Size.x);
	}

	private bool IsOutsideMapBounds(Vector2 tile)
	{
		return tile.x < 0 || tile.y < 0 || tile.x >= rectangleBound.Size.x || tile.y >= rectangleBound.Size.y;
	}

	private void AddTraversableTiles(Godot.Collections.Array traversableTiles)
	{
		foreach (Vector2 tile in traversableTiles)
		{
			astar.AddPoint(GetTileId(tile), tile);
		}
	}

	private void ConnectTraversableTiles(Godot.Collections.Array traversableTiles)
	{
		foreach (Vector2 tile in traversableTiles)
		{
			int tileId = GetTileId(tile);

			for (int x_or_y = 0; x_or_y < 2; ++x_or_y)
			{
				for (int add = 0; add < 2; ++add)
				{
					Vector2 target = new Vector2(tile.x, tile.y);
					
					if (x_or_y == 0)
					{
						target.y += 1 - 2 * add;
					}
					else
					{
						target.x += 1 - 2 * add;
					}

					if (IsOutsideMapBounds(target))
					{
						continue;
					}

					int targetId = GetTileId(target);

					if (!astar.HasPoint(targetId))
					{
						continue;
					}

					astar.ConnectPoints(tileId, targetId);
				}
			}
		}
	}

	
}
