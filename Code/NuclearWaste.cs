using Godot;
using System;

namespace SnakeGame
{
	public partial class NuclearWaste : Collectable
	{
		public override void Collect(Snake snake)
		{
			Level.Current.DestroySnake();
		}
	}
}