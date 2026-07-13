using Godot;
using System.Collections.Generic;

public partial class AudioPlayer : AudioStreamPlayer
{
    private AudioStreamPlaybackPolyphonic playback;

    private long currentSongStreamId;

    private string currentSongName;

    private Dictionary<string, AudioStream> musicLibrary = new()
    {
        { "morning", GD.Load<AudioStream>("res://assets/music/morning.mp3") },
        { "nostalgia", GD.Load<AudioStream>("res://assets/music/nostalgia.mp3") },
        { "power_of_the_wind", GD.Load<AudioStream>("res://assets/music/power_of_the_wind.mp3") },
        { "relaxing_piano_music", GD.Load<AudioStream>("res://assets/music/relaxing_piano_music.mp3") },
        { "typhoon_parade", GD.Load<AudioStream>("res://assets/music/typhoon_parade.mp3") },
        { "woosh", GD.Load<AudioStream>("res://assets/music/woosh.mp3") },
    };

    private Dictionary<string, AudioStream> soundEffectLibrary = new()
    {
        { "click", GD.Load<AudioStream>("res://assets/sound_effects/click.wav") },
        { "land", GD.Load<AudioStream>("res://assets/sound_effects/land.mp3") },
    };

    public override void _Ready()
    {
        Stream = new AudioStreamPolyphonic();
        Play();
        playback = GetStreamPlayback() as AudioStreamPlaybackPolyphonic;
    }

    public void PlayMusic(string songName)
    {
        if (currentSongName != null && currentSongName == songName)
        {
            return;
        }
        if (musicLibrary.TryGetValue(songName, out var song))
        {
            StopMusic();
            currentSongStreamId = playback.PlayStream(song);
            currentSongName = songName;
        }
        else
        {
            GD.Print($"Song not found: {songName}");
        }
    }

    public void StopMusic()
    {
        if (playback.IsStreamPlaying(currentSongStreamId))
        {
            playback.StopStream(currentSongStreamId);
        }
    }

    public void PlaySound(string soundName)
    {
        if (soundEffectLibrary.TryGetValue(soundName, out var sound))
        {
            playback.PlayStream(sound);
        }
        else
        {
            GD.Print($"Sound not found: {soundName}");
        }
    }
}
