using Godot;

namespace StellaGame;

public partial class AudioPlayer : AudioStreamPlayer
{
    [Export]
    public StateManager State { get; set; }

    public AudioStreamPlaybackPolyphonic Playback
    {
        get
        {
            if (_playback == null)
            {
                Play();
                _playback = GetStreamPlayback() as AudioStreamPlaybackPolyphonic;
            }
            return _playback;
        }
    }

    private long currentSongStreamId = 0;

    private AudioStreamPlaybackPolyphonic _playback = null;

    public void Load()
    {
        StopMusic();
        PlayMusic(State.ActiveSlot.CurrentSong, false);
    }

    public void PlayMusic(string songName, bool save = true)
    {
        if (songName == null)
        {
            StopMusic();
        }
        if (save && State.ActiveSlot.CurrentSong != null && State.ActiveSlot.CurrentSong == songName)
        {
            return;
        }
        if (AssetLibrary.GetMusic(songName, out var song))
        {
            StopMusic();
            currentSongStreamId = Playback.PlayStream(song);

            if (save)
            {
                State.ActiveSlot.CurrentSong = songName;
            }
        }
        else
        {
            GD.Print($"Song '{songName}' not found.");
        }
    }

    public void StopMusic()
    {
        if (Playback != null && Playback.IsStreamPlaying(currentSongStreamId))
        {
            Playback.StopStream(currentSongStreamId);
        }
    }

    public void PlaySound(string soundName)
    {
        if (AssetLibrary.GetSound(soundName, out var sound))
        {
            Playback.PlayStream(sound);
        }
        else
        {
            GD.Print($"Sound '{soundName}' not found.");
        }
    }
}
