using Godot;
using System.Collections.Generic;

public partial class AudioPlayer : AudioStreamPlayer
{
    private AudioStreamPlaybackPolyphonic playback;

    private long currentSongId;

    private string currentSongName;

    private Dictionary<string, AudioStream> musicLibrary = new()
    {
        { "dream", GD.Load<AudioStream>("res://assets/music/dream.mp3") },
        { "typhoon_parade", GD.Load<AudioStream>("res://assets/music/typhoon_parade.mp3") },
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
        if (musicLibrary.TryGetValue(songName, out AudioStream song))
        {
            playback.StopStream(currentSongId);
            currentSongId = playback.PlayStream(song);
            currentSongName = songName;
        }
        else
        {
            GD.Print($"Song not found: {songName}");
        }
    }

    public void StopMusic()
    {
        playback.Stop();
    }
}
