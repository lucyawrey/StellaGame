using Godot;
using System.Collections.Generic;

namespace StellaGame;

public static class AssetLibrary
{
    private static Dictionary<string, AudioStream> music = new()
    {
        { "morning", GD.Load<AudioStream>("res://assets/music/morning.mp3") },
        { "nostalgia", GD.Load<AudioStream>("res://assets/music/nostalgia.mp3") },
        { "power_of_the_wind", GD.Load<AudioStream>("res://assets/music/power_of_the_wind.mp3") },
        { "relaxing_piano_music", GD.Load<AudioStream>("res://assets/music/relaxing_piano_music.mp3") },
        { "typhoon_parade", GD.Load<AudioStream>("res://assets/music/typhoon_parade.mp3") },
        { "woosh", GD.Load<AudioStream>("res://assets/music/woosh.mp3") },
    };

    private static Dictionary<string, AudioStream> sound = new()
    {
        { "click", GD.Load<AudioStream>("res://assets/sound_effects/click.wav") },
        { "land", GD.Load<AudioStream>("res://assets/sound_effects/land.mp3") },
    };

    private static Dictionary<string, Texture2D> backgrounds = new()
    {
        { "cafe", GD.Load<Texture2D>("res://assets/backgrounds/cafe.png") },
        { "bedroom", GD.Load<Texture2D>("res://assets/backgrounds/bedroom.png") },
    };

    private static Dictionary<string, Texture2D> sprites = new()
    {
        { "stella", GD.Load<Texture2D>("res://assets/sprites/stella.png") },
        { "azalea", GD.Load<Texture2D>("res://assets/sprites/azalea.png") },
        { "giovanni", GD.Load<Texture2D>("res://assets/sprites/giovanni.png") },
    };

    private static Dictionary<string, Theme> themes = new()
    {
        { "base", GD.Load<Theme>("res://src/resources/themes/base.tres") },
        { "void", GD.Load<Theme>("res://src/resources/themes/void.tres") },
    };

    private static Dictionary<string, FontFile> fonts = new()
    {
        { "base", GD.Load<FontFile>("res://assets/fonts/PoppinsSemiBold.ttf") }
    };

    public static string BaseFontPath { get; }

    public static bool GetMusic(string songName, out AudioStream song)
    {
        if (songName == null)
        {
            song = null;
            return false;
        }
        return music.TryGetValue(songName, out song);
    }

    public static bool GetSound(string soundName, out AudioStream sound)
    {
        if (soundName == null)
        {
            sound = null;
            return false;
        }
        return AssetLibrary.sound.TryGetValue(soundName, out sound);
    }

    public static bool GetBackground(string imageName, out Texture2D image)
    {
        if (imageName == null)
        {
            image = null;
            return false;
        }
        return backgrounds.TryGetValue(imageName, out image);
    }

    public static bool GetSprite(string imageName, out Texture2D image)
    {
        if (imageName == null)
        {
            image = null;
            return false;
        }
        return sprites.TryGetValue(imageName, out image);
    }

    public static bool GetFont(string fontName, out FontFile font)
    {
        if (fontName == null)
        {
            font = null;
            return false;
        }
        return fonts.TryGetValue(fontName, out font);
    }

    public static int GetBaseFontSize()
    {
        return 42;
    }

    public static bool GetTheme(string themeName, out Theme theme)
    {
        if (themeName == null)
        {
            theme = null;
            return false;
        }
        return themes.TryGetValue(themeName, out theme);
    }
}
