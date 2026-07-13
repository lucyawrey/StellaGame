using System.Text.RegularExpressions;
using Godot;
using GodotInk;
using Ink.Parsed;

public partial class Story : Node
{
	[Export]
	private InkStory story;

	[Export]
	private AudioPlayer audioPlayer;

	[Export]
	private RichTextLabel dialogueLabel;

	[Export]
	private PanelContainer nameBox;

	[Export]
	private RichTextLabel nameLabel;

	[Export]
	private Button[] choiceButtons;

	[Export]
	private BoxContainer choicesBox;

	private bool continueIsLocked = false;

	private readonly Regex characterRegex = new Regex(@"^([^\s]+):\s(.*?)$");

	public override void _Ready()
	{
		// Ink external function setup
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

		// Choice button callback set up
		for (var i = 0; i < choiceButtons.Length; i++)
		{
			var choice = i;
			choiceButtons[i].Pressed += delegate
			{
				story.ChooseChoiceIndex(choice);
				Continue();
			};
		}

		Callable.From(Continue).CallDeferred();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (!continueIsLocked && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				audioPlayer.PlaySound("click");
				Continue();
			}
		}
	}

	private void setChoiceMode(bool choiceMode)
	{
		dialogueLabel.Visible = !choiceMode;
		choicesBox.Visible = choiceMode;
	}

	private void hideControl(bool hide, Control control)
	{
		if (hide)
		{
			control.Modulate = new Color(1, 1, 1, 0);
			control.MouseFilter = Control.MouseFilterEnum.Ignore;
		}
		else
		{
			control.Modulate = new Color(1, 1, 1, 1);
			control.MouseFilter = Control.MouseFilterEnum.Stop;
		}
	}

	private void Continue()
	{
		continueIsLocked = true;

		if (story.GetCanContinue())
		{
			string content = story.Continue().Trim();
			if (string.IsNullOrWhiteSpace(content))
			{
				Continue();
				return;
			}
			setChoiceMode(false);
			string name = "";
			var match = characterRegex.Match(content);
			if (match.Success)
			{
				name = match.Groups[1].Value;
				content = match.Groups[2].Value;
			}
			if (content == "_")
			{
				content = string.Empty;
			}
			nameBox.Visible = !string.IsNullOrWhiteSpace(name);

			dialogueLabel.Text = content;
			nameLabel.Text = name;
			continueIsLocked = false;
		}
		else if (story.CurrentChoices.Count > 0)
		{
			nameBox.Visible = false;
			foreach (var button in choiceButtons)
			{
				hideControl(true, button);
			}
			foreach (InkChoice choice in story.CurrentChoices)
			{
				setChoiceMode(true);
				if (choice.Index < 4)
				{
					hideControl(false, choiceButtons[choice.Index]);
					choiceButtons[choice.Index].Text = choice.Text;
				}
			}
		}
		else
		{
			continueIsLocked = false;
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
