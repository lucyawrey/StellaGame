using System.Collections.Generic;
using System.Text.Json;
using Godot;
using StellaGame;

public partial class StateManager : Node
{
    [Export]
    private StoryManager storyManager;

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

        public JsonDocument InkStory { get; set; }

        public string CurrentSongName { get; set; } = null;

        public string CurrentBackgroundImageName { get; set; } = "none";

        public Dictionary<string, Vector2> CurrentSpriteGroup { get; set; } = new();
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
