using System.Collections.Generic;
using Godot;
using Tomlyn;

public partial class CharacterManager : Node
{
    private readonly string charactersPath = "res://story/characters.toml";

    private Dictionary<string, ImportCharacter> importCharacters;

    private Dictionary<string, Character> characterCache = new Dictionary<string, Character>();

    private readonly string defaultColor = "#FFFFFF";

    private readonly string baseCharacterId = "Base";

    private readonly string narratorCharacterId = "Narrator";

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
        public string NameColor { get; set; }
        public string TextColor { get; set; }
        public string Font { get; set; }
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

    private Character ConvertToCharacter(ImportCharacter importCharacter, string nameFallback = null)
    {
        var character = new Character
        {
            Name = importCharacter.Name ?? nameFallback ?? "",
            NameColor = importCharacter.NameColor ?? defaultColor,
            TextColor = importCharacter.TextColor ?? defaultColor,
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

    private Character MergeCharacters(List<Character> characters)
    {
        var mergedCharacter = new Character();
        foreach (var character in characters)
        {
            if (character == null) continue;
            mergedCharacter.Name = character.Name ?? mergedCharacter.Name;
            mergedCharacter.NameColor = character.NameColor ?? mergedCharacter.NameColor;
            mergedCharacter.TextColor = character.TextColor ?? mergedCharacter.TextColor;
            mergedCharacter.Font = character.Font ?? mergedCharacter.Font;
            mergedCharacter.FontSize = character.FontSize != 0 ? character.FontSize : mergedCharacter.FontSize;
            mergedCharacter.TextSpeed = character.TextSpeed != 0 ? character.TextSpeed : mergedCharacter.TextSpeed;
            mergedCharacter.Prefix = character.Prefix ?? mergedCharacter.Prefix;
            mergedCharacter.Suffix = character.Suffix ?? mergedCharacter.Suffix;
            mergedCharacter.TextSound = character.TextSound ?? mergedCharacter.TextSound;
            mergedCharacter.TextSoundPitch = character.TextSoundPitch != 0 ? character.TextSoundPitch : mergedCharacter.TextSoundPitch;
            mergedCharacter.TextSoundSpacing = character.TextSoundSpacing != 0 ? character.TextSoundSpacing : mergedCharacter.TextSoundSpacing;
            mergedCharacter.Fullscreen = character.Fullscreen || mergedCharacter.Fullscreen;
            mergedCharacter.Theme = character.Theme ?? mergedCharacter.Theme;
        }
        return mergedCharacter;
    }

    public Character GetCharacter(string characterId)
    {
        Character character;
        var initialCharacterId = characterId ?? "";
        if (characterCache.TryGetValue(initialCharacterId, out character))
        {
            return character;
        }

        if (string.IsNullOrWhiteSpace(characterId))
        {
            characterId = narratorCharacterId;
        }
        characterId = $"{baseCharacterId}.{characterId}";

        var splitId = characterId.Split('.');
        var characters = new List<Character>();
        ImportCharacter lastImportCharacter = null;
        var i = 0;
        foreach (var id in splitId)
        {
            ImportCharacter importCharacter = null;
            if ((lastImportCharacter?.Forms != null && lastImportCharacter.Forms.TryGetValue(id, out importCharacter)) || importCharacters.TryGetValue(id, out importCharacter))
            {
                characters.Add(ConvertToCharacter(importCharacter, i == 1 ? id : null));
                lastImportCharacter = importCharacter;
            }
            else
            {
                GD.PrintErr($"Character not found: {id}");
            }
            i++;
        }
        character = MergeCharacters(characters);
        characterCache[initialCharacterId] = character;
        return character;
    }
}
