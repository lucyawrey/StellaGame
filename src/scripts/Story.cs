using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using Godot;
using GodotInk;

namespace StellaGame;

public partial class Story : Node
{
	[Export]
	public Game Game { get; set; }

	[Export]
	public InkStory InkStory { get; set; }

	[Export]
	public Control UserInterface { get; set; }

	[Export]
	public Menu Menu { get; set; }

	[Export]
	public RichTextLabel FullscreenDialogueLabel { get; set; }

	[Export]
	public RichTextLabel DialogueLabel { get; set; }

	[Export]
	public RichTextLabel NameLabel { get; set; }

	[Export]
	public PanelContainer FullscreenBox { get; set; }

	[Export]
	public PanelContainer DialogueBox { get; set; }

	[Export]
	public SpriteGroup SpriteGroup { get; set; }

	[Export]
	public PanelContainer NameBox { get; set; }

	[Export]
	public Godot.Collections.Array<Button> ChoiceButtons { get; set; }

	[Export]
	public Godot.Collections.Array<Button> ActChoiceButtons { get; set; }

	[Export]
	public BoxContainer ChoicesBox { get; set; }

	[Export]
	public Control Content { get; set; }

	[Export]
	public BoxContainer ActChoicesBox { get; set; }

	[Export]
	public BackgroundImage BackgroundImage { get; set; }

	[Export]
	public ProgressStar ProgressStar { get; set; }

	public CharacterManager CharacterManager { get; set; }

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

	private Button buttonToFocus = null;

	private Dictionary<string, int> actChoiceIndexMap = new() { { "Act", -1 }, { "Talk", -1 }, { "Move", -1 } };

	private Dictionary<string, Button> actChoiceButtonMap = new();

	public enum DialogueBoxMode
	{
		Dialogue,
		Choices,
		ActChoices
	}

	public Story()
	{
		CharacterManager = new CharacterManager();
	}

	public override void _Ready()
	{
		// Ink external function setup
		InkStory.BindExternalFunction("show", new Callable(this, "Show"));
		InkStory.BindExternalFunction("hide", new Callable(this, "Hide"));
		InkStory.BindExternalFunction("clear", new Callable(this, "Clear"));
		InkStory.BindExternalFunction("background", new Callable(this, "Background"));
		InkStory.BindExternalFunction("play_music", new Callable(this, "PlayMusic"));
		InkStory.BindExternalFunction("stop_music", new Callable(this, "StopMusic"));
		InkStory.BindExternalFunction("play_sound", new Callable(this, "PlaySound"));
		InkStory.BindExternalFunction("auto", new Callable(this, "Auto"));
		InkStory.BindExternalFunction("delay", new Callable(this, "Delay"));
		InkStory.BindExternalFunction("add_item", new Callable(this, "AddItem"));
		InkStory.BindExternalFunction("remove_item", new Callable(this, "RemoveItem"));

		InkStory.BindExternalFunction("get_date", new Callable(this, "GetDate"));
		InkStory.BindExternalFunction("get_debug_mode", new Callable(this, "GetDebugMode"));
		InkStory.BindExternalFunction("get_item", new Callable(this, "GetItem"));

		// Choice button callback set up
		foreach (var choiceButton in ActChoiceButtons)
		{
			actChoiceButtonMap[choiceButton.Text] = choiceButton;
			choiceButton.Pressed += delegate
			{
				InkStory.ChooseChoiceIndex(actChoiceIndexMap[choiceButton.Text]);
				advanceCounter++;
			};
			choiceButton.MouseEntered += choiceButton.GrabFocus;
		}
		for (var i = 0; i < ChoiceButtons.Count; i++)
		{
			var choice = i;
			ChoiceButtons[i].Pressed += delegate
			{
				InkStory.ChooseChoiceIndex(choice);
				advanceCounter++;
			};
			ChoiceButtons[i].MouseEntered += ChoiceButtons[i].GrabFocus;
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Menu.Visible)
		{
			return;
		}

		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				InputAdvance();
			}
		}
		else
		{
			if (@event.IsActionPressed("ui_accept") && !@event.IsEcho())
			{
				InputAdvance();
			}
			if (@event.IsActionPressed("ui_cancel") && !@event.IsEcho())
			{
				Content.Visible = false;
				Menu.Open();
			}
			if (@event.IsActionPressed("reload") && !@event.IsEcho())
			{
				Game.State.LoadGame(deleteSlot: true);
			}
			if (@event.IsActionPressed("skip"))
			{
				isSkipMode = true;
			}
			if (@event.IsActionReleased("skip"))
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
				var spaces = DialogueLabel.GetParsedText().Count(" ", DialogueLabel.VisibleCharacters, Math.Min(DialogueLabel.VisibleCharacters + charactersPerTick, DialogueLabel.GetParsedText().Length));
				if (textSoundCharacterCounter <= 0)
				{
					Game.AudioPlayer.Playback.PlayStream(currentCharacter.TextSound, 0, -2.0f, currentCharacter.TextSoundPitch);
					textSoundCharacterCounter = currentCharacter.TextSoundSpacing;
				}
				textSoundCharacterCounter -= charactersPerTick - spaces;

				DialogueLabel.VisibleCharacters += charactersPerTick;
				visibleCharactersTimer = 0;
			}
			else
			{
				visibleCharactersTimer += delta;
			}
		}
		else if (!ProgressStar.Visible && DialogueLabel.IsVisibleInTree())
		{
			ProgressStar.Visible = true;
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

	public void Load()
	{
		if (Game.State.ActiveSlot.InkStory != null)
		{
			InkStory.LoadState(JsonSerializer.Serialize(Game.State.ActiveSlot.InkStory));
		}
		else
		{
			InkStory.ResetState();
		}
		advanceCounter++;
	}

	public void Open()
	{
		Content.Visible = true;
		if (buttonToFocus != null)
		{
			buttonToFocus.GrabFocus();
		}
	}

	private void InputAdvance()
	{
		if (IsContinueLocked())
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
		return DialogueLabel.VisibleCharacters != -1 && DialogueLabel.IsVisibleInTree() && DialogueLabel.Text.Length > 0 && DialogueLabel.VisibleCharacters <= DialogueLabel.GetParsedText().Length;
	}

	private void FinishTextPrinting()
	{
		DialogueLabel.VisibleCharacters = -1;
	}

	private bool IsContinueLocked()
	{
		return isDelayed || !Content.IsVisibleInTree() || (!DialogueLabel.IsVisibleInTree() && !FullscreenDialogueLabel.IsVisibleInTree());
	}

	private void SetDialogueBoxMode(DialogueBoxMode mode)
	{
		switch (mode)
		{
			case DialogueBoxMode.Dialogue:
				DialogueLabel.Visible = true;
				ChoicesBox.Visible = false;
				ActChoicesBox.Visible = false;
				break;
			case DialogueBoxMode.Choices:
				DialogueLabel.Visible = false;
				ChoicesBox.Visible = true;
				ActChoicesBox.Visible = false;
				break;
			case DialogueBoxMode.ActChoices:
				DialogueLabel.Visible = false;
				ChoicesBox.Visible = false;
				ActChoicesBox.Visible = true;
				break;
		}
	}

	private void HideControl(bool hide, Control control)
	{
		if (hide)
		{
			control.Modulate = new Color(1, 1, 1, 0);
			control.MouseFilter = Control.MouseFilterEnum.Ignore;
			control.FocusMode = Control.FocusModeEnum.None;
		}
		else
		{
			control.Modulate = new Color(1, 1, 1, 1);
			control.MouseFilter = Control.MouseFilterEnum.Stop;
			control.FocusMode = Control.FocusModeEnum.All;
		}
	}

	private void Continue()
	{
		ProgressStar.Visible = false;
		Content.Visible = true;
		if (InkStory.GetCanContinue())
		{
			string rawText;
			if (delayedContent == null)
			{
				// Autosave
				Game.State.SaveGame();

				rawText = InkStory.Continue().Trim();
			}
			else
			{
				rawText = delayedContent;
			}

			while (actionQueue.TryDequeue(out var action))
			{
				if (action is StoryAction.Show showAction)
				{
					SpriteGroup.AddSprite(showAction.ImageName, showAction.Position);
				}
				else if (action is StoryAction.Hide hideAction)
				{
					SpriteGroup.RemoveSprite(hideAction.ImageName);
				}
				else if (action is StoryAction.Clear)
				{
					SpriteGroup.RemoveAllSprites();
				}
				else if (action is StoryAction.Background backgroundAction)
				{
					BackgroundImage.Set(backgroundAction.ImageName, true);
				}
				else if (action is StoryAction.PlayMusic playMusicAction)
				{
					Game.AudioPlayer.PlayMusic(playMusicAction.SongName);
				}
				else if (action is StoryAction.StopMusic)
				{
					Game.AudioPlayer.StopMusic();
				}
				else if (action is StoryAction.PlaySound playSoundAction)
				{
					Game.AudioPlayer.PlaySound(playSoundAction.SoundName);
				}
				else if (action is StoryAction.Auto auto)
				{
					advanceCounter += auto.Count;
				}
				else if (action is StoryAction.Delay delayAction)
				{
					isDelayed = true;
					delayClock = delayAction.Time;
					delayedContent = rawText;
					return;
				}
				else if (action is StoryAction.AddItem addItemAction)
				{
					Game.State.ActiveSlot.Inventory.AddItem(addItemAction.ItemName, addItemAction.Quantity);
				}
				else if (action is StoryAction.RemoveItem removeItemAction)
				{
					Game.State.ActiveSlot.Inventory.RemoveItem(removeItemAction.ItemName, removeItemAction.Quantity);
				}
			}

			delayedContent = null;

			if (string.IsNullOrWhiteSpace(rawText))
			{
				Continue();
				return;
			}

			SetDialogueBoxMode(DialogueBoxMode.Dialogue);
			string characterId = "";
			var match = REGEX.Match(rawText);
			if (match.Success)
			{
				characterId = match.Groups[1].Value;
				rawText = match.Groups[2].Value;
			}

			if (rawText == "_")
			{
				rawText = "";
			}

			currentCharacter = CharacterManager.GetCharacter(characterId);

			var nameText = string.IsNullOrWhiteSpace(currentCharacter.Name) ? "" : $"[color={currentCharacter.NameColor}]{currentCharacter.Name}[/color]";
			var fontPath = currentCharacter.Font.ResourcePath;

			var font = fontPath != AssetLibrary.BaseFontPath ? $"[font={fontPath}]" : "";
			var fontSize = currentCharacter.FontSize != AssetLibrary.GetBaseFontSize() ? $"[font_size={currentCharacter.FontSize}]" : "";
			var fullscreenNameText = currentCharacter.Fullscreen && !string.IsNullOrWhiteSpace(nameText) ? $"{nameText}: " : "";
			var dialogueText = $"{font}{fontSize}[color={currentCharacter.TextColor}]{fullscreenNameText}{currentCharacter.Prefix}{rawText}{currentCharacter.Suffix}";

			if (currentCharacter.Fullscreen)
			{
				FullscreenBox.Visible = true;
				DialogueBox.Visible = false;
				NameBox.Visible = false;

				FullscreenDialogueLabel.AppendText($"\n{dialogueText}");
			}
			else
			{
				DialogueLabel.VisibleCharacters = 0;
				FullscreenBox.Visible = false;
				DialogueBox.Visible = true;
				NameBox.Visible = !string.IsNullOrWhiteSpace(nameText);

				NameLabel.Text = nameText;
				DialogueLabel.Text = dialogueText;
			}

			// Set theme for character
			if (currentCharacter.Theme != null && Content.Theme != currentCharacter.Theme)
			{
				Content.Theme = currentCharacter.Theme;
			}

			// Complete text if content is hidden
			if (!Content.Visible)
			{
				FinishTextPrinting();
			}
		}
		else if (InkStory.CurrentChoices.Count > 0)
		{
			NameBox.Visible = false;

			foreach (var button in ActChoiceButtons)
			{
				button.Disabled = true;
			}
			foreach (var key in actChoiceIndexMap.Keys)
			{
				actChoiceIndexMap[key] = -1;
			}
			foreach (var button in ChoiceButtons)
			{
				HideControl(true, button);
			}
			var focusIsSet = false;
			foreach (InkChoice choice in InkStory.CurrentChoices)
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
					if (!focusIsSet)
					{
						buttonToFocus = actChoiceButtonMap[choice.Text];
						buttonToFocus.GrabFocus();
						focusIsSet = true;
					}
				}
				else
				{
					SetDialogueBoxMode(DialogueBoxMode.Choices);
					HideControl(false, ChoiceButtons[choice.Index]);
					ChoiceButtons[choice.Index].Text = choice.Text;
					if (!focusIsSet)
					{
						buttonToFocus = ChoiceButtons[choice.Index];
						buttonToFocus.GrabFocus();
						focusIsSet = true;
					}
				}
			}
		}
		else
		{
			Game.State.LoadGame(deleteSlot: true);
			Game.State.SaveGame();
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
		return "October 7th, 2097";
	}

	private bool GetDebugMode(bool isDebugMode)
	{
		return isDebugMode;
	}

	private bool GetItem(string itemName, int quantity)
	{
		return Game.State.ActiveSlot.Inventory.CheckForItem(itemName, quantity) != -1;
	}
}
