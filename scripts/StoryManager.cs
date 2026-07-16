using System.Text.RegularExpressions;
using Godot;
using Godot.Collections;
using GodotInk;

public partial class StoryManager : Node
{
	[Export]
	private InkStory story;

	[Export]
	private CharacterManager characterManager;

	[Export]
	private Control userInterface;

	[Export]
	private AudioPlayer audioPlayer;

	[Export]
	private RichTextLabel fullscreenDialogueLabel;

	[Export]
	private RichTextLabel dialogueLabel;

	[Export]
	private RichTextLabel nameLabel;

	[Export]
	private PanelContainer fullscreenBox;

	[Export]
	private PanelContainer dialogueBox;

	[Export]
	private PanelContainer nameBox;

	[Export]
	private Array<Button> choiceButtons;

	[Export]
	private Array<Button> actChoiceButtons;

	[Export]
	private BoxContainer choicesBox;

	[Export]
	private BoxContainer actChoicesBox;

	[Export]
	private BackgroundImage backgroundImage;

	private readonly Regex characterRegex = new Regex(@"^([^\s]+):\s(.*?)$");

	private bool isAutoMode = false;

	private bool continueIsLocked = false;

	private CharacterManager.Character currentCharacter = null;

	private double visibleCharactersTimer = 0.0;

	private int textSoundCharacterCounter = 0;

	private long textSoundStreamId = 0;

	private Dictionary<string, int> actChoiceIndexMap = new() { { "Act", -1 }, { "Talk", -1 }, { "Move", -1 } };

	private Dictionary<string, Button> actChoiceButtonMap = new();

	public enum DialogueBoxMode
	{
		Dialogue,
		Choices,
		ActChoices
	}

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
		foreach (var choiceButton in actChoiceButtons)
		{
			actChoiceButtonMap[choiceButton.Text] = choiceButton;
			choiceButton.Pressed += delegate
			{
				story.ChooseChoiceIndex(actChoiceIndexMap[choiceButton.Text]);
				Continue();
			};
		}
		for (var i = 0; i < choiceButtons.Count; i++)
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
				Continue();
			}
		}
		else if (@event is InputEventKey keyEvent)
		{
			if (!continueIsLocked && keyEvent.Pressed && keyEvent.Keycode == Key.Space)
			{
				Continue();
			}
			if (keyEvent.Pressed && keyEvent.Keycode == Key.Shift)
			{
				isAutoMode = true;
			}
			else if (!keyEvent.Pressed && keyEvent.Keycode == Key.Shift)
			{
				isAutoMode = false;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (dialogueLabel.VisibleCharacters != -1 && visibleCharactersTimer >= 1.0 / currentCharacter.TextSpeed)
		{
			var currentTextCharacter = dialogueLabel.Text[dialogueLabel.VisibleCharacters];
			if (!char.IsWhiteSpace(currentTextCharacter))
			{
				if (textSoundCharacterCounter <= 0)
				{
					audioPlayer.Playback.StopStream(textSoundStreamId);
					textSoundStreamId = audioPlayer.Playback.PlayStream(currentCharacter.TextSound, 0, -2.0f, currentCharacter.TextSoundPitch);
					textSoundCharacterCounter = currentCharacter.TextSoundSpacing;
				}
				textSoundCharacterCounter--;
			}

			dialogueLabel.VisibleCharacters++;
			visibleCharactersTimer = 0;
		}

		if (dialogueLabel.VisibleCharacters >= dialogueLabel.Text.Length)
		{
			audioPlayer.Playback.StopStream(textSoundStreamId);

			dialogueLabel.VisibleCharacters = -1;
		}

		visibleCharactersTimer += delta;

		if (!continueIsLocked && isAutoMode)
		{
			Continue();
		}
	}

	private void SetDialogueBoxMode(DialogueBoxMode mode)
	{
		switch (mode)
		{
			case DialogueBoxMode.Dialogue:
				dialogueLabel.Visible = true;
				choicesBox.Visible = false;
				actChoicesBox.Visible = false;
				break;
			case DialogueBoxMode.Choices:
				dialogueLabel.Visible = false;
				choicesBox.Visible = true;
				actChoicesBox.Visible = false;
				break;
			case DialogueBoxMode.ActChoices:
				dialogueLabel.Visible = false;
				choicesBox.Visible = false;
				actChoicesBox.Visible = true;
				break;
		}
	}

	private void HideControl(bool hide, Control control)
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
			SetDialogueBoxMode(DialogueBoxMode.Dialogue);

			string characterId = "";
			var match = characterRegex.Match(content);
			if (match.Success)
			{
				characterId = match.Groups[1].Value;
				content = match.Groups[2].Value;
			}

			if (content == "_")
			{
				content = "";
			}

			currentCharacter = characterManager.GetCharacter(characterId);

			var nameText = string.IsNullOrWhiteSpace(currentCharacter.Name) ? "" : $"[color={currentCharacter.NameColor}]{currentCharacter.Name}[/color]";

			var font = currentCharacter.Font != AssetManager.GetBaseFont() ? $"[font={currentCharacter.Font}]" : "";
			var fontSize = currentCharacter.FontSize != AssetManager.GetBaseFontSize() ? $"[font_size={currentCharacter.FontSize}]" : "";
			var fullscreenNameText = currentCharacter.Fullscreen && !string.IsNullOrWhiteSpace(nameText) ? $"{nameText}: " : "";
			var dialogueText = $"{font}{fontSize}[color={currentCharacter.TextColor}]{fullscreenNameText}{currentCharacter.Prefix}{content}{currentCharacter.Suffix}";

			if (currentCharacter.Fullscreen)
			{
				dialogueLabel.VisibleCharacters = -1;
				fullscreenBox.Visible = true;
				dialogueBox.Visible = false;
				nameBox.Visible = false;

				fullscreenDialogueLabel.AppendText($"\n{dialogueText}");
			}
			else
			{
				dialogueLabel.VisibleCharacters = 0;
				fullscreenBox.Visible = false;
				dialogueBox.Visible = true;
				nameBox.Visible = !string.IsNullOrWhiteSpace(nameText);

				nameLabel.Text = nameText;
				dialogueLabel.Text = dialogueText;
			}

			// Set theme for character
			if (currentCharacter.Theme != null && userInterface.Theme != currentCharacter.Theme)
			{
				userInterface.Theme = currentCharacter.Theme;
			}

			continueIsLocked = false;
		}
		else if (story.CurrentChoices.Count > 0)
		{
			dialogueLabel.VisibleCharacters = -1;
			nameBox.Visible = false;

			foreach (var button in actChoiceButtons)
			{
				button.Disabled = true;
			}
			foreach (var key in actChoiceIndexMap.Keys)
			{
				actChoiceIndexMap[key] = -1;
			}
			foreach (var button in choiceButtons)
			{
				HideControl(true, button);
			}
			foreach (InkChoice choice in story.CurrentChoices)
			{
				if (choice.Index >= 4)
				{
					GD.PrintErr("There are more than 4 choices, which is too many to be displayed.");
					break;
				}
				if (actChoiceIndexMap.Keys.Contains(choice.Text))
				{
					SetDialogueBoxMode(DialogueBoxMode.ActChoices);
					actChoiceIndexMap[choice.Text] = choice.Index;
					actChoiceButtonMap[choice.Text].Disabled = false;
				}
				else
				{
					SetDialogueBoxMode(DialogueBoxMode.Choices);
					HideControl(false, choiceButtons[choice.Index]);
					choiceButtons[choice.Index].Text = choice.Text;
				}
			}
		}
		else
		{
			GetTree().Quit();
		}
	}

	private void Clear()
	{

	}
	private void Background(string imageName)
	{
		backgroundImage.Set(imageName);
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
