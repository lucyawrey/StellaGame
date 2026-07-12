using Godot;
using GodotInk;

public partial class Story : Node
{
	[Export]
	private InkStory story;

	[Export]
	private VBoxContainer ui;

	[Export]
	private AudioPlayer audioPlayer;

	public override void _Ready()
	{
		story.BindExternalFunction("clear", new Callable(this, "Clear"));
		story.BindExternalFunction("background", new Callable(this, "Background"));
		story.BindExternalFunction("play_music", new Callable(this, "PlayMusic"));
		story.BindExternalFunction("stop_music", new Callable(this, "StopMusic"));
		story.BindExternalFunction("play_sound", new Callable(this, "PlaySound"));
		story.BindExternalFunction("auto", new Callable(this, "Auto"));
		story.BindExternalFunction("delay", new Callable(this, "Delay"));
		story.BindExternalFunction("get_date", new Callable(this, "GetDate"));
		story.BindExternalFunction("get_debug_mode", new Callable(this, "GetDebugMode"));
		story.BindExternalFunction("add_item", new Callable(this, "AddItem"));
		story.BindExternalFunction("check_item", new Callable(this, "CheckItem"));
		story.BindExternalFunction("remove_item", new Callable(this, "RemoveItem"));

		Callable.From(Continue).CallDeferred();
	}

	private void FreeChildren()
	{
		foreach (Node child in ui.GetChildren())
		{
			child.QueueFree();
		}
	}

	private void Continue()
	{
		FreeChildren();

		if (story.GetCanContinue())
		{
			string content = story.Continue().Trim();

			if (string.IsNullOrWhiteSpace(content))
			{
				Continue();
				return;
			}
			if (content == "_")
			{
				content = string.Empty;
			}

			Label text = new() { Text = content };
			ui.AddChild(text);
			Button button = new() { Text = "Continue" };
			button.Pressed += Continue;
			ui.AddChild(button);
			audioPlayer.PlaySound("click");
		}
		else if (story.CurrentChoices.Count > 0)
		{
			foreach (InkChoice choice in story.CurrentChoices)
			{
				Button button = new() { Text = choice.Text };
				button.Pressed += delegate
				{
					story.ChooseChoiceIndex(choice.Index);
					Continue();
				};
				ui.AddChild(button);
				audioPlayer.PlaySound("click");
			}
		}
		else
		{
			Label text = new() { Text = "THE END" };
			ui.AddChild(text);
		}
	}

	private void Clear()
	{

	}
	private void Background(string imageName)
	{
		if (imageName == "none")
		{
			return;
		}
	}
	private void PlayMusic(string songName)
	{
		audioPlayer.PlayMusic(songName);
	}
	private void StopMusic()
	{
		audioPlayer.StopMusic();
	}
	private void PlaySound(string soundName)
	{
		audioPlayer.PlaySound(soundName);
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
