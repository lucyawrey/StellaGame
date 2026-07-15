using Godot;
using System.Collections.Generic;

public partial class AudioPlayer : AudioStreamPlayer
{
    public AudioStreamPlaybackPolyphonic Playback { get; set; }

    private long currentSongStreamId;

    private string currentSongName;

    public override void _Ready()
    {
        Stream = new AudioStreamPolyphonic();
        Play();
        Playback = GetStreamPlayback() as AudioStreamPlaybackPolyphonic;
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
            currentSongStreamId = Playback.PlayStream(song);
            currentSongName = songName;
        }
        else
        {
            GD.Print($"Song '{songName}' not found.");
        }
    }

    public void StopMusic()
    {
        if (Playback.IsStreamPlaying(currentSongStreamId))
        {
            Playback.StopStream(currentSongStreamId);
        }
    }

    public void PlaySound(string soundName)
    {
        if (AssetManager.GetSound(soundName, out var sound))
        {
            Playback.PlayStream(sound);
        }
        else
        {
            GD.Print($"Sound '{soundName}' not found.");
        }
    }
}
