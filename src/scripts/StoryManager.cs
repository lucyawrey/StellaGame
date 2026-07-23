using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Godot;
using GodotInk;

namespace StellaGame;

public partial class StoryManager : Node
{
	[Export]
	public InkStory Story { get; set; }

	[Export]
	private StateManager state;

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
	private SpriteGroup spriteGroup;

	[Export]
	private PanelContainer nameBox;

	[Export]
	private Godot.Collections.Array<Button> choiceButtons;

	[Export]
	private Godot.Collections.Array<Button> actChoiceButtons;

	[Export]
	private BoxContainer choicesBox;

	[Export]
	private BoxContainer actChoicesBox;

	[Export]
	private BackgroundImage backgroundImage;

	private static readonly Regex REGEX = new Regex(@"^([^\s]+):\s(.*?)$");

	private Queue<StoryAction> actionQueue = new();

	private bool isSkipMode = false;

	private int advanceCounter = 0;

	private bool isDelayed = false;

	private string delayedContent = null;

	private double delayClock = 0;

	private CharacterManager.Character currentCharacter = null;

	private double visibleCharactersTimer = 0.0;

	private int textSoundCharacterCounter = 0;

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
		Story.BindExternalFunction("show", new Callable(this, "Show"));
		Story.BindExternalFunction("hide", new Callable(this, "Hide"));
		Story.BindExternalFunction("clear", new Callable(this, "Clear"));
		Story.BindExternalFunction("background", new Callable(this, "Background"));
		Story.BindExternalFunction("play_music", new Callable(this, "PlayMusic"));
		Story.BindExternalFunction("stop_music", new Callable(this, "StopMusic"));
		Story.BindExternalFunction("play_sound", new Callable(this, "PlaySound"));
		Story.BindExternalFunction("auto", new Callable(this, "Auto"));
		Story.BindExternalFunction("delay", new Callable(this, "Delay"));
		Story.BindExternalFunction("add_item", new Callable(this, "AddItem"));
		Story.BindExternalFunction("remove_item", new Callable(this, "RemoveItem"));

		Story.BindExternalFunction("get_date", new Callable(this, "GetDate"));
		Story.BindExternalFunction("get_debug_mode", new Callable(this, "GetDebugMode"));
		Story.BindExternalFunction("get_item", new Callable(this, "GetItem"));

		// Choice button callback set up
		foreach (var choiceButton in actChoiceButtons)
		{
			actChoiceButtonMap[choiceButton.Text] = choiceButton;
			choiceButton.Pressed += delegate
			{
				Story.ChooseChoiceIndex(actChoiceIndexMap[choiceButton.Text]);
				advanceCounter++;
			};
		}
		for (var i = 0; i < choiceButtons.Count; i++)
		{
			var choice = i;
			choiceButtons[i].Pressed += delegate
			{
				Story.ChooseChoiceIndex(choice);
				advanceCounter++;
			};
		}

		advanceCounter++;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				InputAdvance();
			}
		}
		else if (@event is InputEventKey keyEvent)
		{
			if (keyEvent.Pressed && !keyEvent.IsEcho() && keyEvent.Keycode == Key.Space)
			{
				InputAdvance();
			}
			if (keyEvent.Pressed && keyEvent.Keycode == Key.Shift)
			{
				isSkipMode = true;
			}
			if (!keyEvent.Pressed && keyEvent.Keycode == Key.Shift)
			{
				isSkipMode = false;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// Text printing effect
		if (IsTextPrinting() && currentCharacter != null)
		{
			var charactersPerTick = 1 + currentCharacter.TextSpeed / 60; // Tick rate is 60
			var textSpeed = currentCharacter.TextSpeed / charactersPerTick;

			if (visibleCharactersTimer >= 1.0 / textSpeed)
			{
				var spaces = dialogueLabel.GetParsedText().Count(" ", dialogueLabel.VisibleCharacters, Math.Min(dialogueLabel.VisibleCharacters + charactersPerTick, dialogueLabel.GetParsedText().Length));
				if (textSoundCharacterCounter <= 0)
				{
					audioPlayer.Playback.PlayStream(currentCharacter.TextSound, 0, -2.0f, currentCharacter.TextSoundPitch);
					textSoundCharacterCounter = currentCharacter.TextSoundSpacing;
				}
				textSoundCharacterCounter -= charactersPerTick - spaces;

				dialogueLabel.VisibleCharacters += charactersPerTick;
				visibleCharactersTimer = 0;
			}
			else
			{
				visibleCharactersTimer += delta;
			}
		}

		// Handle delay action
		if (isDelayed)
		{
			if (delayClock <= 0)
			{
				isDelayed = false;
				advanceCounter++;
			}
			else
			{
				delayClock -= delta;
			}
		}
		// Continue story if advance counter is set and game is not delayed
		else if (advanceCounter > 0 && !IsTextPrinting())
		{
			advanceCounter--;
			Continue();
		}

		// Skip mode
		if (isSkipMode)
		{
			InputAdvance();
		}
	}

	private void InputAdvance()
	{
		if (IsInputLocked())
		{
			return;
		}
		if (IsTextPrinting())
		{
			FinishTextPrinting();
		}
		else if (advanceCounter == 0)
		{
			advanceCounter++;
		}
	}

	private bool IsTextPrinting()
	{
		return dialogueLabel.IsVisibleInTree() && dialogueLabel.Text.Length > 0 && dialogueLabel.VisibleCharacters <= dialogueLabel.GetParsedText().Length;
	}

	private void FinishTextPrinting()
	{
		dialogueLabel.VisibleCharacters = dialogueLabel.GetParsedText().Length;
	}

	private bool IsInputLocked()
	{
		return isDelayed;
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
		if (Story.GetCanContinue())
		{
			string content;
			if (delayedContent == null)
			{
				// Autosave
				state.SaveGame();

				content = Story.Continue().Trim();
			}
			else
			{
				content = delayedContent;
			}

			while (actionQueue.TryDequeue(out var action))
			{
				if (action is StoryAction.Show showAction)
				{
					spriteGroup.AddSprite(showAction.ImageName, showAction.Position);
				}
				else if (action is StoryAction.Hide hideAction)
				{
					spriteGroup.RemoveSprite(hideAction.ImageName);
				}
				else if (action is StoryAction.Clear)
				{
					spriteGroup.RemoveAllSprites();
				}
				else if (action is StoryAction.Background backgroundAction)
				{
					backgroundImage.Set(backgroundAction.ImageName, state);
				}
				else if (action is StoryAction.PlayMusic playMusicAction)
				{
					audioPlayer.PlayMusic(playMusicAction.SongName);
				}
				else if (action is StoryAction.StopMusic)
				{
					audioPlayer.StopMusic();
				}
				else if (action is StoryAction.PlaySound playSoundAction)
				{
					audioPlayer.PlaySound(playSoundAction.SoundName);
				}
				else if (action is StoryAction.Auto auto)
				{
					advanceCounter += auto.Count;
				}
				else if (action is StoryAction.Delay delayAction)
				{
					isDelayed = true;
					delayClock = delayAction.Time;
					delayedContent = content;
					return;
				}
				else if (action is StoryAction.AddItem addItemAction)
				{
					// TODO add item action
				}
				else if (action is StoryAction.RemoveItem removeItemAction)
				{
					// TODO remove item aciton
				}
			}

			delayedContent = null;

			if (string.IsNullOrWhiteSpace(content))
			{
				Continue();
				return;
			}

			SetDialogueBoxMode(DialogueBoxMode.Dialogue);
			string characterId = "";
			var match = REGEX.Match(content);
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

			var font = currentCharacter.Font != AssetLibrary.GetBaseFont() ? $"[font={currentCharacter.Font}]" : "";
			var fontSize = currentCharacter.FontSize != AssetLibrary.GetBaseFontSize() ? $"[font_size={currentCharacter.FontSize}]" : "";
			var fullscreenNameText = currentCharacter.Fullscreen && !string.IsNullOrWhiteSpace(nameText) ? $"{nameText}: " : "";
			var dialogueText = $"{font}{fontSize}[color={currentCharacter.TextColor}]{fullscreenNameText}{currentCharacter.Prefix}{content}{currentCharacter.Suffix}";

			if (currentCharacter.Fullscreen)
			{
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
		}
		else if (Story.CurrentChoices.Count > 0)
		{
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
			foreach (InkChoice choice in Story.CurrentChoices)
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

	private void Show(string imageName, string position)
	{
		actionQueue.Enqueue(new StoryAction.Show() { ImageName = imageName, Position = position });
	}

	private void Hide(string imageName)
	{
		actionQueue.Enqueue(new StoryAction.Hide() { ImageName = imageName });
	}

	private void Clear()
	{
		actionQueue.Enqueue(new StoryAction.Clear());
	}

	private void Background(string imageName)
	{
		actionQueue.Enqueue(new StoryAction.Background() { ImageName = imageName });
	}

	private void PlayMusic(string songName)
	{
		actionQueue.Enqueue(new StoryAction.PlayMusic() { SongName = songName });
	}

	private void StopMusic()
	{
		actionQueue.Enqueue(new StoryAction.StopMusic());
	}

	private void PlaySound(string soundName)
	{
		actionQueue.Enqueue(new StoryAction.PlaySound() { SoundName = soundName });
	}

	private void Auto(int count)
	{
		actionQueue.Enqueue(new StoryAction.Auto() { Count = count });
	}

	private void Delay(float time)
	{
		actionQueue.Enqueue(new StoryAction.Delay() { Time = time });
	}

	private void AddItem(string itemName, int quantity)
	{
		actionQueue.Enqueue(new StoryAction.AddItem() { ItemName = itemName, Quantity = quantity });
	}

	private void RemoveItem(string itemName, int quantity)
	{
		actionQueue.Enqueue(new StoryAction.RemoveItem() { ItemName = itemName, Quantity = quantity });
	}

	private string GetDate(int gameDay)
	{
		return "October 4th, 2097";
	}

	private bool GetDebugMode(bool isDebugMode)
	{
		return isDebugMode;
	}

	private bool GetItem(string itemName, int quantity)
	{
		return true;
	}
}
