using Godot;
using System;

namespace SnakeGame
{
	public partial class Calculator : Node
	{
		public override void _Process(double delta)
		{
			GD.Print($"A:{5} + B:{4} = {Sum(5, 4)}");
			GD.Print($"A:{5} * B:{4} = {Multiply(5, 4)}");
			GD.Print($"A:{1} + B:{4} = {1 + 4}");
			GD.Print($"A:{1} - B:{4} = {1 - 4}");
			GD.Print($"A:{1} * B:{4} = {1 * 4}");
			GD.Print($"A:{1} / B:{4} = {1 / 4}");
		}

		public int Sum(int a, int b)
		{
			return a + b;
		}

		public int Multiply(int a, int b)
		{
			return a * b;
		}

		public int Deduct(int a, int b)
		{
			return a - b;
		}

		public int Divide(int a, int b)
		{
			return a / b;
		}

	}
}