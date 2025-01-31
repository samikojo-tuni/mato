using Godot;
using System;

namespace SnakeGame
{
	public partial class Snake : Node2D
	{
		// Enum on integer, jonka arvot ovat nimetty.
		public enum Direction
		{
			None = 0,
			Up,
			Down,
			Left,
			Right,
		}

		[Export] private float _speed = 1;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			Direction direction = ReadInput();
			Move(direction, (float)delta);
		}

		private void Move(Direction direction, float delta)
		{
			Vector2 directionVector = GetDirectionVector(direction);
			Translate(directionVector * _speed * delta);
		}

		private Vector2 GetDirectionVector(Direction direction)
		{
			switch (direction)
			{
				case Direction.Up: return Vector2.Up;
				case Direction.Down: return Vector2.Down;
				case Direction.Right: return Vector2.Right;
				case Direction.Left: return Vector2.Left;
				default: return Vector2.Zero; // Mikä tahansa muu case.
			}
		}

		private Direction ReadInput()
		{
			Direction direction = Direction.None;

			if (Input.IsActionPressed(Config.MoveUpAction))
			{
				direction = Direction.Up;
			}
			else if (Input.IsActionPressed(Config.MoveDownAction))
			{
				direction = Direction.Down;
			}
			else if (Input.IsActionPressed(Config.MoveLeftAction))
			{
				direction = Direction.Left;
			}
			else if (Input.IsActionPressed(Config.MoveRightAction))
			{
				direction = Direction.Right;
			}

			return direction;
		}
	}
}