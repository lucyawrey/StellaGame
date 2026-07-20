using System.Text.Json;
using Godot;
using StellaGame;

public partial class StateManager : Node
{
    private const string SAVE_PATH = "user://save.json";

    private const string SLOT_PATH = "user://slot_";

    private const string JSON_EXT = ".json";

    private static readonly JsonSerializerOptions JSON_OPTIONS = new() { WriteIndented = true };

    [Export]
    private StoryManager storyManager;

    public class SaveData
    {
        public int ActiveSlotId { get; set; } = 0;
    }

    public class SlotData
    {
        public int Id { get; set; } = 0;
        public JsonDocument InkStory { get; set; }
    }

    public SaveData Save { get; set; } = new();

    public SlotData ActiveSlot { get; set; } = new();

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
