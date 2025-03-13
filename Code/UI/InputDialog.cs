using Godot;

namespace SnakeGame.UI
{
	public partial class InputDialog : DialogWindow
	{
		[Export] private LineEdit _inputField = null;

		public string GetInput()
		{
			return _inputField.Text;
		}
	}
}