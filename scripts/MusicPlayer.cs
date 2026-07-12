using Godot;
using System;
using System.Collections.Generic;

public partial class MusicPlayer : AudioStreamPlayer
{
    public Dictionary<string, AudioStream> MusicLibrary = new()
    {
        { "dream", GD.Load<AudioStream>("res://assets/music/dream.mp3") },
    };
    public void PlaySongByName(string songName)
    {
        if (MusicLibrary.TryGetValue(songName, out var songStream))
        {
            if (Stream == songStream)
            {
                return;
            }
            Stream = songStream;
            Play();
        }
        else
        {
            GD.Print($"Missing MusicLibrary entry: {songName}");
        }
    }
}
