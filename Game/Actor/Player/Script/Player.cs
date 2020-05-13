using Godot;
using System;
using System.Collections.Generic;

public class Player : Position2D
{
	public enum State {
		Idle,
		Follow
	};

	private List<Vector2> path;
	private Vector2 targetPointNext;
	private State state;

	// Be carefult with the ratio of ArrivedDistance and Speed
	[Export]
	private const float ArrivedDistance = 4.0f;
	[Export]
	private const float Speed = 200.0f;
	
	public override void _Ready()
	{
		ChangeState(State.Idle);

		GetTree()
			.Root
			.GetNode("Node2D/PathTile")
			.Connect("PlayerSetPath", this, "SetPath");
	}

	[Signal]
	delegate void PlayerMapFindPath(Vector2 from, Vector2 to);

	public void SetPath(List<Vector2> newPath)
	{
		path = new List<Vector2>(newPath);
		GD.Print("Size", path.Count, "-", newPath.Count);

		GD.Print("Change state");
		ChangeState(State.Follow);

		GD.Print("State");
		GD.Print(state);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton)
		{
			if (Input.IsKeyPressed((int) KeyList.Shift))
			{
				GlobalPosition = GetGlobalMousePosition();
			}
			else
			{
				EmitSignal("PlayerMapFindPath", Position, GetGlobalMousePosition());
			}
		}
	}

	public override void _Process(float delta)
	{
		if (state != State.Follow)
		{
			GetNode<AnimatedSprite>("PlayerAnimation").Play("Idle");
			return;
		}

		bool isArrived = Move(targetPointNext);

		if (isArrived)
		{
			Position = path[0];
			path.RemoveAt(0);
			if (path.Count == 0)
			{
				ChangeState(State.Idle);
				return;
			}
			targetPointNext = path[0];
		}
	}

	private void ChangeState(State stateNew)
	{
		if (stateNew == State.Follow)
		{
			// path = GetTree()
			// 	.Root
			// 	.GetNode<PathTile>("PathTile")
			// .GetTilePath(Position, targetPosition);

			GD.Print("Try follow");
			GD.Print(path);
			GD.Print(path.Count);
			
			if (path.Count <= 1)
			{
				GD.Print("Idle");
				GD.Print(path.Count);
				state = State.Idle;
				return;
			}

			targetPointNext = path[1];
			path.RemoveAt(0);
		}
		state = stateNew;

		GD.Print("State changed");
	}

	private void ChangeSpriteDirection(Vector2 direction)
	{
		if (direction.x > 0 && Math.Abs(direction.x) > Math.Abs(direction.y))
		{
			GetNode<AnimatedSprite>("PlayerAnimation").Play("Right");
		}
		else if (direction.x < 0 && Math.Abs(direction.x) > Math.Abs(direction.y))
		{
			GetNode<AnimatedSprite>("PlayerAnimation").Play("Left");
		}
		else if (direction.y > 0 && Math.Abs(direction.x) < Math.Abs(direction.y))
		{
			GetNode<AnimatedSprite>("PlayerAnimation").Play("Down");
		}
		else if (direction.y < 0 && Math.Abs(direction.x) < Math.Abs(direction.y))
		{
			GetNode<AnimatedSprite>("PlayerAnimation").Play("Up");
		}
		else
		{
			GetNode<AnimatedSprite>("PlayerAnimation").Play("Idle");
		}
	}

	public bool Move(Vector2 to)
	{
		Vector2 velocity = (to - Position).Normalized();

		Position += velocity * Speed * GetProcessDeltaTime();

		ChangeSpriteDirection(velocity);

		return Position.DistanceTo(to) < ArrivedDistance;
	}
}
