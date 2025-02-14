using Godot;
using System;

namespace SnakeGame
{
	public partial class Calculator : Node
	{
		public override void _Process(double delta)
		{
			GD.Print($"A:{1} + B:{4} = {1 + 4}");
			GD.Print($"A:{1} - B:{4} = {1 - 4}");
			GD.Print($"A:{1} * B:{4} = {1 * 4}");
			GD.Print($"A:{1} / B:{4} = {1 / 4}");
		}
	}
}