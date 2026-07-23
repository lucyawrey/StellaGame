using Godot;

namespace StellaGame;

public partial class BackgroundImage : TextureRect
{
    public void Set(string imageName, StateManager state)
    {
        if (imageName == "none")
        {
            Texture = null;
        }
        else if (AssetLibrary.GetBackground(imageName, out var image))
        {
            Texture = image;
            state.ActiveSlot.CurrentBackgroundImageName = imageName;
        }
        else
        {
            GD.Print($"Background image '{imageName}' not found.");
        }
    }
}
