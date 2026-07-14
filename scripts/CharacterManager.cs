using System.Collections.Generic;
using Godot;
using Tomlyn;

public partial class CharacterManager : Node
{
    private readonly string charactersPath = "res://story/characters.toml";

    private Dictionary<string, ImportCharacter> importCharacters;

    private Dictionary<string, Character> characterCache = new Dictionary<string, Character>();

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
        public string Name { get; set; }
        public Color NameColor { get; set; }
        public Color TextColor { get; set; }
        public FontFile Font { get; set; }
        public int FontSize { get; set; }
        public float TextSpeed { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public AudioStream TextSound { get; set; }
        public float TextSoundPitch { get; set; }
        public int TextSoundSpacing { get; set; }
        public bool Fullscreen { get; set; }
        public Theme Theme { get; set; }
    }

    public override void _Ready()
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
            Font = AssetManager.GetFont(importCharacter.Font, out var font) ? font : null,
            FontSize = importCharacter.FontSize,
            TextSpeed = importCharacter.TextSpeed,
            Prefix = importCharacter.Prefix,
            Suffix = importCharacter.Suffix,
            TextSound = AssetManager.GetSound(importCharacter.TextSound, out var sound) ? sound : null,
            TextSoundPitch = importCharacter.TextSoundPitch,
            TextSoundSpacing = importCharacter.TextSoundSpacing,
            Fullscreen = importCharacter.Fullscreen,
            Theme = AssetManager.GetTheme(importCharacter.Theme, out var theme) ? theme : null
        };
        return character;
    }

    public Character GetCharacter(string characterId)
    {
        if (characterCache.TryGetValue(characterId, out var character))
        {
            return character;
        }
        var splitId = characterId.Split('.');
        if (splitId.Length > 1)
        {
            return ConvertToGameCharacter(importCharacters[splitId[0]], splitId[0]);
        }
        else
        {
            return ConvertToGameCharacter(importCharacters[characterId], characterId);
        }
    }
}
