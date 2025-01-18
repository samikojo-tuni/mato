using Godot;
using System;

namespace SnakeGame
{
	public partial class TestScript : Node
	{
		private int _luku = 1;
		private Calculator _calculator = null;

		[Export] private int _luku2 = 0;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			GD.Print($"Calculator: {_calculator}");

			_calculator = GetNode<Calculator>("Calculator");

			GD.Print($"Calculator: {_calculator}");
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			_luku = _calculator.Sum(_luku, _luku2);
			GD.Print($"Luku: {_luku}");
		}
	}
}