using Godot;

namespace SnakeGame
{
	public abstract partial class Collectable : Sprite2D, ICellOccupier
	{
		public CellOccupierType Type => CellOccupierType.Collectable;

		public Vector2I GridPosition { get; set; }

		public bool SetPosition(Vector2I gridPosition)
		{
			// TODO: Toteuta minut!
			return false;
		}

		public abstract void Collect(Snake snake);
	}
}