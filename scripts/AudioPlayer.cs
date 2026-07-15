using Godot;
using System.Collections.Generic;

public partial class AudioPlayer : AudioStreamPlayer
{
    private AudioStreamPlaybackPolyphonic playback;

    private long currentSongStreamId;

    private string currentSongName;

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
        if (AssetManager.GetMusic(songName, out var song))
        {
            StopMusic();
            currentSongStreamId = playback.PlayStream(song);
            currentSongName = songName;
        }
        else
        {
            GD.Print($"Song '{songName}' not found.");
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
        if (AssetManager.GetSound(soundName, out var sound))
        {
            playback.PlayStream(sound);
        }
        else
        {
            GD.Print($"Sound '{soundName}' not found.");
        }
    }
}
