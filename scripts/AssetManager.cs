using Godot;
using System.Collections.Generic;

public static class AssetManager
{
    private static Dictionary<string, AudioStream> musicLibrary = new()
    {
        { "morning", GD.Load<AudioStream>("res://assets/music/morning.mp3") },
        { "nostalgia", GD.Load<AudioStream>("res://assets/music/nostalgia.mp3") },
        { "power_of_the_wind", GD.Load<AudioStream>("res://assets/music/power_of_the_wind.mp3") },
        { "relaxing_piano_music", GD.Load<AudioStream>("res://assets/music/relaxing_piano_music.mp3") },
        { "typhoon_parade", GD.Load<AudioStream>("res://assets/music/typhoon_parade.mp3") },
        { "woosh", GD.Load<AudioStream>("res://assets/music/woosh.mp3") },
    };

    private static Dictionary<string, AudioStream> soundLibrary = new()
    {
        { "click", GD.Load<AudioStream>("res://assets/sound_effects/click.wav") },
        { "land", GD.Load<AudioStream>("res://assets/sound_effects/land.mp3") },
    };

    private static Dictionary<string, Texture2D> backgroundLibrary = new()
    {
        { "cafe", GD.Load<Texture2D>("res://assets/backgrounds/cafe.png") },
        { "bedroom", GD.Load<Texture2D>("res://assets/backgrounds/bedroom.png") },
    };

    private static Dictionary<string, FontFile> fontLibrary = new()
    {
        { "base", GD.Load<FontFile>("res://assets/fonts/PoppinsSemiBold.ttf") }
    };

    //theme
    private static Dictionary<string, Theme> themeLibrary = new()
    {
        { "base", GD.Load<Theme>("res://assets/themes/base.tres") },
    };

    public static bool GetMusic(string songName, out AudioStream song)
    {
        return musicLibrary.TryGetValue(songName, out song);
    }

    public static bool GetSound(string soundName, out AudioStream sound)
    {
        return soundLibrary.TryGetValue(soundName, out sound);
    }

    public static bool GetBackground(string imageName, out Texture2D image)
    {
        return backgroundLibrary.TryGetValue(imageName, out image);
    }

    public static bool GetFont(string fontName, out FontFile font)
    {
        return fontLibrary.TryGetValue(fontName, out font);
    }

    public static bool GetTheme(string themeName, out Theme theme)
    {
        return themeLibrary.TryGetValue(themeName, out theme);
    }
}
