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

    public void PlayMusic(string songName)
    {
        if (state.ActiveSlot.CurrentSongName != null && state.ActiveSlot.CurrentSongName == songName)
        {
            return;
        }
        if (AssetLibrary.GetMusic(songName, out var song))
        {
            state.ActiveSlot.CurrentSongName = songName;

            StopMusic();
            currentSongStreamId = Playback.PlayStream(song);
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
