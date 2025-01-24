using Godot;
using System;

namespace SnakeGame
{
	public partial class HelloFibonacci : Node2D
	{
		private int _luku1 = 0;
		private int _luku2 = 1;
		private int _frame = 0;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			GD.Print("Hello, World!");
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if (_frame == 0)
			{
				_frame++;
				GD.Print($"Frame: {_frame}: {_luku1}");
				return;
			}
			else if (_frame == 1)
			{
				_frame++;
				GD.Print($"Frame: {_frame}: {_luku2}");
				return;
			}

			_frame++;
			int summa = _luku1 + _luku2;
			if (_luku2 < 1000)
			{
				GD.Print($"Frame: {_frame}: {summa}");
				// Alla oleva rivi tulostaa saman stringin kuin ylempi
				// GD.Print("Frame: " + _frame + " :" + _luku1);

				int temp = _luku2;
				_luku2 = summa;
				_luku1 = temp;
			}
		}
	}
}