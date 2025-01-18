using Godot;
using System;

namespace SnakeGame
{
	public partial class Calculator : Node
	{
		public int Sum(int a, int b)
		{
			return a + b;
		}

		public int Deduct(int a, int b)
		{
			return a - b;
		}
	}
}