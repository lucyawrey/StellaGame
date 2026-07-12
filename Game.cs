using Godot;
using GodotInk;

public partial class Game : VBoxContainer
{
	[Export]
	private InkStory story;

	public override void _Ready()
	{
		ContinueStory();
	}

	private void ContinueStory()
	{
		GridContainer grid = new() { Columns = 2 };

		GridContainer left = new();
		foreach (Node child in GetChildren())
		{
			child.QueueFree();
		}
		Label content = new() { Text = story.ContinueMaximally() };
		left.AddChild(content);

		GridContainer right = new();
		foreach (InkChoice choice in story.CurrentChoices)
		{
			Button button = new() { Text = choice.Text };
			button.Pressed += delegate
			{
				story.ChooseChoiceIndex(choice.Index);
				ContinueStory();
			};
			right.AddChild(button);
		}

		grid.AddChild(left);
		grid.AddChild(right);
		AddChild(grid);
	}
}
