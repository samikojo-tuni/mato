using Godot;
using System.Collections.Generic;

namespace SnakeGame
{
	public partial class SnakePart : Node2D, ICellOccupier
	{
		public enum SnakePartType
		{
			None = 0,
			Head,
			Body,
			Tail
		}

		public CellOccupierType Type { get { return CellOccupierType.Snake; } }

		[Export] public SnakePartType PartType { get; set; } = SnakePartType.None;
		public Vector2I GridPosition { get; private set; }

		public Grid LevelGrid
		{
			get { return Level.Current.Grid; }
		}

		// Madon rotaatio asteina.
		public static Dictionary<Snake.Direction, int> Directions = new Dictionary<Snake.Direction, int>()
		{
			{ Snake.Direction.Up, 0},
			{ Snake.Direction.Down, 180},
			{ Snake.Direction.Left, 270},
			{ Snake.Direction.Right, 90},
		};

		/// <summary>
		/// Asettaa madon osan gridPositionia vastaavaan sijaintiin levelissä. Varaa gridiltä sijaintia vastaavan solun.
		/// </summary>
		/// <param name="gridPosition">Sijainti gridin koodrinaatiostssa.</param>
		/// <returns>True, jos sijainnnin asettaminen onnistuu, false muuten.</returns>
		public bool SetPosition(Vector2I gridPosition)
		{
			if (LevelGrid.GetWorldPosition(gridPosition, out Vector2 worldPosition) &&
				LevelGrid.OccupyCell(this, gridPosition))
			{
				// Käännä madon osa oikein.
				Snake.Direction movingDirection = GetMovingDirection(GridPosition, gridPosition);
				if (movingDirection != Snake.Direction.None)
				{
					RotationDegrees = Directions[movingDirection];
				}

				// Aseta madon sijainti gridille ja leveliin
				GridPosition = gridPosition;
				Position = worldPosition;
				return true;
			}

			// Madon uusi sijainti ei ole gridillä (mato yrittää liikkua levelin ulkopuolelle) tai solun varaus
			// ei onnistu (este).
			return false;
		}

		private Snake.Direction GetMovingDirection(Vector2I originalPosition, Vector2I newPosition)
		{
			if (originalPosition.X < newPosition.X)
			{
				return Snake.Direction.Right;
			}
			if (originalPosition.X > newPosition.X)
			{
				return Snake.Direction.Left;
			}
			if (originalPosition.Y < newPosition.Y)
			{
				return Snake.Direction.Down;
			}
			if (originalPosition.Y > newPosition.Y)
			{
				return Snake.Direction.Up;
			}

			// Ei liikuta mihinkään.
			return Snake.Direction.None;
		}
	}
}