using System.Collections.Generic;
using System.Text.Json;
using Godot;
using StellaGame;

public partial class StateManager : Node
{
    [Export]
    private StoryManager storyManager;

    [Export]
    private AudioPlayer audioPlayer;

    [Export]
    private SpriteGroup spriteGroup;

    [Export]
    private BackgroundImage backgroundImage;

    public bool GameIsLoaded { get; set; } = false;

    public SaveData Save { get; set; } = new();

    public SlotData ActiveSlot { get; set; } = new();

    private const string SAVE_PATH = "user://save.json";

    private const string SLOT_PATH = "user://slot_";

    private const string JSON_EXT = ".json";

    private static readonly JsonSerializerOptions JSON_OPTIONS = new() { WriteIndented = true };

    public class SaveData
    {
        public int ActiveSlotId { get; set; } = 0;
    }

    public class SlotData
    {
        public int Id { get; set; } = 0;

        public JsonDocument InkStory { get; set; } = null;

        public string CurrentSong { get; set; } = null;

        public string CurrentBackgroundImage { get; set; } = null;

        public Dictionary<string, string> CurrentSpriteGroup { get; set; } = new();
    }

    public void LoadGame(int switchToSlot = -1, bool deleteSlot = false)
    {
        if (deleteSlot)
        {
            Save = new();
            ActiveSlot = new();
        }
        else
        {
            // Load Main Save
            if (FileAccess.FileExists(SAVE_PATH))
            {
                using var saveFile = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Read);
                var saveJson = saveFile.GetAsText();
                Save = JsonSerializer.Deserialize<SaveData>(saveJson);
            }
            else
            {
                GD.Print($"File not found at {SAVE_PATH}, creating new save.");
            }

            // Load Active Slot
            if (switchToSlot >= 0)
            {
                Save.ActiveSlotId = switchToSlot;
            }
            var slotPath = $"{SLOT_PATH}{Save.ActiveSlotId}{JSON_EXT}";
            if (FileAccess.FileExists(slotPath))
            {
                using var slotFile = FileAccess.Open(slotPath, FileAccess.ModeFlags.Read);
                var slotJson = slotFile.GetAsText();
                ActiveSlot = JsonSerializer.Deserialize<SlotData>(slotJson);
            }
            else
            {
                GD.Print($"File not found at {slotPath}, creating new save.");
            }
        }

        // Load everything;
        storyManager.Load();
        audioPlayer.Load();
        backgroundImage.Load(this);
        spriteGroup.Load();
        GameIsLoaded = true;
    }

    public void SaveGame()
    {
        // Save
        string saveJsonString = JsonSerializer.Serialize(Save, JSON_OPTIONS);
        using var saveFile = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Write);

        if (saveFile != null)
        {
            saveFile.StoreLine(saveJsonString);
            GD.Print("Game saved.");
        }

        // Active Slot
        ActiveSlot.InkStory = JsonDocument.Parse(storyManager.Story.SaveState());

        string slotJsonString = JsonSerializer.Serialize(ActiveSlot, JSON_OPTIONS);
        using var slotFile = FileAccess.Open($"{SLOT_PATH}{ActiveSlot.Id}{JSON_EXT}", FileAccess.ModeFlags.Write);

        if (slotFile != null)
        {
            slotFile.StoreLine(slotJsonString);
            GD.Print($"Slot {ActiveSlot.Id} saved");
        }
    }
}
