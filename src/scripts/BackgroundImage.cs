using Godot;

namespace StellaGame;

public partial class BackgroundImage : TextureRect
{
    public void Load(StateManager state)
    {
        Set(state.ActiveSlot.CurrentBackgroundImage, state);
    }

    public void Set(string imageName, StateManager state)
    {
        if (imageName == "none" || imageName == null)
        {
            Texture = null;
        }
        else if (AssetLibrary.GetBackground(imageName, out var image))
        {
            Texture = image;
            state.ActiveSlot.CurrentBackgroundImage = imageName;
        }
        else
        {
            GD.Print($"Background image '{imageName}' not found.");
        }
    }
}
