using Godot;

namespace StellaGame;

public partial class AudioPlayer : AudioStreamPlayer
{
    [Export]
    private StateManager state;

    public AudioStreamPlaybackPolyphonic Playback { get; set; }

    private long currentSongStreamId;

    public override void _Ready()
    {
        Play();
        Playback = GetStreamPlayback() as AudioStreamPlaybackPolyphonic;
    }

    public void Load()
    {
        PlayMusic(state.ActiveSlot.CurrentSong, false);
    }

    public void PlayMusic(string songName, bool save = true)
    {
        if (songName == null)
        {
            StopMusic();
        }
        if (state.ActiveSlot.CurrentSong != null && state.ActiveSlot.CurrentSong == songName)
        {
            return;
        }
        if (AssetLibrary.GetMusic(songName, out var song))
        {
            StopMusic();
            currentSongStreamId = Playback.PlayStream(song);

            if (save)
            {
                state.ActiveSlot.CurrentSong = songName;
            }
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
