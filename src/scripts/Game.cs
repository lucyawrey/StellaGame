using Godot;

namespace StellaGame;

public partial class Game : Node
{
    [Export]
    public Story Story { get; set; }

    [Export]
    public AudioPlayer AudioPlayer { get; set; }

    [Export]
    public SpriteGroup SpriteGroup { get; set; }

    [Export]
    public BackgroundImage BackgroundImage { get; set; }

    public State State { get; set; }

    public Game()
    {
        State = new(this);
    }
}
