using Godot;

namespace SnakeGame
{
	public partial class TestScript : Node2D
	{
		[Export] private float _speed = 1;
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			GD.Print($"Paikallinen sijainti: {Position}");
			GD.Print($"Globaali sijainti: {GlobalPosition}");

			GlobalPosition = new Vector2(50, 50);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{

			Vector2 movement = Vector2.Down;

			// Lisätään Globaaliin sijaintiin 1 y-koordinaatin suunnassa joka framella.
			// Kappale liikkuu yhden pikselin alaspäin / frame.
			// GlobalPosition += movement;
			// GlobalPosition = GlobalPosition + movement;

			// Liikutetaan yksi pikseli sekunnisssa.
			// GlobalPosition += movement * (float)delta;

			// Liikutetaan nopeuden verran sekunnissa.
			GlobalPosition += movement * (float)delta * _speed;
		}
	}
}