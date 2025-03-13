using System;
using Godot;

namespace SnakeGame
{
	public partial class NuclearWaste : Collectable
	{
		public override void Collect(Snake snake)
		{
			snake.Die();
		}
	}
}