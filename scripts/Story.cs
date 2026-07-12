using Godot;
using GodotInk;

public partial class Story : VBoxContainer
{
	[Export]
	private InkStory story;

	[Export]
	private MusicPlayer musicPlayer;

	public override void _Ready()
	{
		story.BindExternalFunction("clear", new Callable(this, "Clear"));
		story.BindExternalFunction("background", new Callable(this, "Background"));
		story.BindExternalFunction("play_music", new Callable(this, "PlayMusic"));
		story.BindExternalFunction("pause_music", new Callable(this, "PauseMusic"));
		story.BindExternalFunction("play_sound", new Callable(this, "PlaySound"));
		story.BindExternalFunction("auto", new Callable(this, "Auto"));
		story.BindExternalFunction("delay", new Callable(this, "Delay"));
		story.BindExternalFunction("get_date", new Callable(this, "GetDate"));
		story.BindExternalFunction("get_debug_mode", new Callable(this, "GetDebugMode"));
		story.BindExternalFunction("add_item", new Callable(this, "AddItem"));
		story.BindExternalFunction("check_item", new Callable(this, "CheckItem"));
		story.BindExternalFunction("remove_item", new Callable(this, "RemoveItem"));

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

	private void Clear()
	{

	}
	private void Background(string imageName)
	{

	}
	private void PlayMusic(string songName)
	{
		musicPlayer.PlaySongByName(songName);
	}
	private void PauseMusic()
	{

	}
	private void PlaySound(string soundName)
	{

	}
	private void Auto()
	{

	}
	private void Delay(float timeInSeconds)
	{

	}
	private string GetDate(int gameDay)
	{
		return "October 4th, 2097";
	}
	private bool GetDebugMode(bool isDebugMode)
	{
		return isDebugMode;
	}
	private void AddItem(string itemName, int quantity)
	{

	}
	private bool CheckItem(string itemName, int quantity)
	{
		return true;
	}
	private void RemoveItem(string itemName, int quantity)
	{

	}
}
