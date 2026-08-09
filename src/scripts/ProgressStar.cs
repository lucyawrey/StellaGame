using Godot;

namespace StellaGame;

public partial class ProgressStar : Sprite2D
{
    public override void _Process(double delta)
    {
        Rotate((float)delta);
    }
}
