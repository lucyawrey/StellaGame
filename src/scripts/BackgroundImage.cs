using Godot;

namespace StellaGame;

public partial class BackgroundImage : TextureRect
{
    [Export]
    private StateManager state;

    public void Set(string imageName)
    {
        state.ActiveSlot.CurrentBackgroundImageName = imageName;

        if (imageName == "none")
        {
            Texture = null;
        }
        else if (AssetLibrary.GetBackground(imageName, out var image))
        {
            Texture = image;
        }
        else
        {
            GD.Print($"Background image '{imageName}' not found.");
        }
    }
}
