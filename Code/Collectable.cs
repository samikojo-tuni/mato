using Godot;
using Godot.Collections;

namespace SnakeGame
{
	public abstract partial class Collectable : Sprite2D, ICellOccupier
	{
		public CellOccupierType Type => CellOccupierType.Collectable;
		public Grid Grid
		{
			get { return Level.Current.Grid; }
		}

		/// <summary>
		/// Kerättävän esineen sijainti Gridin koordinaatistossa.
		/// </summary>
		public Vector2I GridPosition { get; set; }

		/// <summary>
		/// Asettaa esineen sijainnin gridille.
		/// </summary>
		/// <param name="gridPosition">Sijainti gridillä</param>
		/// <returns>True, jos koordinaatti on laillinen ja sijoittaminen onnistuu. 
		/// False muuten.</returns>
		public bool SetPosition(Vector2I gridPosition)
		{
			if (Grid.GetWorldPosition(gridPosition, out Vector2 worldPosition))
			{
				Position = worldPosition;
				GridPosition = gridPosition;
				return true;
			}

			return false;
		}

		public abstract void Collect(Snake snake);

		public Dictionary Save()
		{
			Dictionary saveData = new Dictionary();
			Dictionary positionData = new Dictionary()
			{
				{ "X", GridPosition.X },
				{ "Y", GridPosition.Y }
			};
			saveData.Add("Position", positionData);

			return saveData;
		}

		public bool Load(Dictionary saveData)
		{
			Dictionary positionData = (Dictionary)saveData["Position"];
			if (positionData != null)
			{
				Vector2I gridPosition = new Vector2I((int)positionData["X"], (int)positionData["Y"]);
				return Grid.OccupyCell(this, gridPosition) && SetPosition(gridPosition);
			}

			return false;
		}
	}
}