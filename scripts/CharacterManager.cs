using System.Collections.Generic;
using Godot;
using Tomlyn;

public partial class CharacterManager : RefCounted
{
    private readonly string charactersPath = "res://story/characters.toml";

    private Dictionary<string, ImportCharacter> importCharacters;

    private Dictionary<string, Character> characters = new Dictionary<string, Character>();

    public class ImportCharacter
    {
        public string Name { get; set; }
        public string NameColor { get; set; }
        public string TextColor { get; set; }
        public string Font { get; set; }
        public int FontSize { get; set; }
        public float TextSpeed { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string TextSound { get; set; }
        public float TextSoundPitch { get; set; }
        public int TextSoundSpacing { get; set; }
        public bool Fullscreen { get; set; }
        public string Theme { get; set; }
        public Dictionary<string, ImportCharacter> Forms { get; set; }
    }

    public class Character
    {
        public required string Name { get; set; }
        public required Color NameColor { get; set; }
        public required Color TextColor { get; set; }
        public required FontFile Font { get; set; }
        public required int FontSize { get; set; }
        public required float TextSpeed { get; set; }
        public required string Prefix { get; set; }
        public required string Suffix { get; set; }
        public required AudioStream TextSound { get; set; }
        public required float TextSoundPitch { get; set; }
        public required int TextSoundSpacing { get; set; }
        public required bool Fullscreen { get; set; }
        public required Theme Theme { get; set; }
    }

    public CharacterManager()
    {
        if (!FileAccess.FileExists(charactersPath))
        {
            GD.PrintErr($"File not found at: {charactersPath}");
            return;
        }
        using var file = FileAccess.Open(charactersPath, FileAccess.ModeFlags.Read);
        string toml = file.GetAsText();

        importCharacters = TomlSerializer.Deserialize<Dictionary<string, ImportCharacter>>(toml);
    }

    private Character ConvertToGameCharacter(ImportCharacter importCharacter, string characterId)
    {
        var character = new Character
        {
            Name = importCharacter.Name ?? characterId,
            NameColor = new Color(importCharacter.NameColor),
            TextColor = new Color(importCharacter.TextColor),
            Font = GD.Load<FontFile>(importCharacter.Font),
            FontSize = importCharacter.FontSize,
            TextSpeed = importCharacter.TextSpeed,
            Prefix = importCharacter.Prefix,
            Suffix = importCharacter.Suffix,
            TextSound = GD.Load<AudioStream>(importCharacter.TextSound),
            TextSoundPitch = importCharacter.TextSoundPitch,
            TextSoundSpacing = importCharacter.TextSoundSpacing,
            Fullscreen = importCharacter.Fullscreen,
            Theme = GD.Load<Theme>(importCharacter.Theme)
        };
        return character;
    }

    public Character GetCharacter(string characterId)
    {
        if (characters.TryGetValue(characterId, out var character))
        {
            return character;
        }
        var splitId = characterId.Split('.');
        if (splitId.Length > 1)
        {
            GD.Print(splitId);
        }
        else
        {
            GD.Print(characterId);
        }
        return null;
    }
}
