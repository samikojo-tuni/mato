using Godot;
using System;

namespace SnakeGame
{
	public partial class ProtoMover : Sprite2D
	{
		[Export] private float _speed = 1.0f;
		[Export] private Vector2 _direction = Vector2.Zero;
		[Export] private bool _moveBetweenPoint = false;
		[Export] private Vector2 _pointA = Vector2.Zero;
		[Export] private Vector2 _pointB = Vector2.Zero;

		private bool _moveTowardsB = false;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			if (_moveBetweenPoint)
			{
				GlobalPosition = _pointA;
				_moveTowardsB = true;
			}
			else
			{
				// Normalisoi suuntavektori eli tee siitä yhden mittainen.
				// Tämä pitää tehdä, jotta nopeuden laskeminen menisi oikein.
				_direction = _direction.Normalized();
			}
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if (_moveBetweenPoint)
			{
				if (_moveTowardsB)
				{
					GlobalPosition = GlobalPosition.MoveToward(_pointB, (float)delta * _speed);
					_moveTowardsB = !GlobalPosition.IsEqualApprox(_pointB);
					// if (GlobalPosition.IsEqualApprox(_pointB))
					// {
					// 	_moveTowardsB = false;
					// }
				}
				else
				{
					GlobalPosition = GlobalPosition.MoveToward(_pointA, (float)delta * _speed);
					_moveTowardsB = GlobalPosition.IsEqualApprox(_pointA);
				}
			}
			else
			{
				Vector2 movement = _speed * _direction * (float)delta;
				GlobalPosition += movement;

				GD.Print($"Nopeus: {(movement / (float)delta).Length()}");
			}
		}
	}
}
