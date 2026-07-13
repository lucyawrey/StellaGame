using System.Collections.Generic;
using Godot;
using Tomlyn;

public partial class CharacterManager : RefCounted
{
    private readonly string charactersPath = "res://story/characters.toml";

    private Dictionary<string, Character> characters;

    public CharacterManager()
    {
        if (!FileAccess.FileExists(charactersPath))
        {
            GD.PrintErr($"File not found at: {charactersPath}");
            return;
        }
        using var file = FileAccess.Open(charactersPath, FileAccess.ModeFlags.Read);
        string toml = file.GetAsText();

        //characters = TomlSerializer.Deserialize<Dictionary<string, Character>>(toml);

        //GD.Print($"Loaded {characters.Count} characters from {charactersPath}");
        //GD.Print($"Characters: {string.Join(", ", characters.Keys)}");
    }
}
