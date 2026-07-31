using Godot;

namespace StellaGame;

public partial class BackgroundImage : TextureRect
{
    public void Load(StateManager state)
    {
        Set("none", state, false);
        Set(state.ActiveSlot.CurrentBackgroundImage, state, false);
    }

    public void Set(string imageName, StateManager state, bool save = true)
    {
        if (imageName == "none" || imageName == null)
        {
            Texture = null;
        }
        else if (AssetLibrary.GetBackground(imageName, out var image))
        {
            Texture = image;
            if (save)
            {
                state.ActiveSlot.CurrentBackgroundImage = imageName;
            }
        }
        else
        {
            GD.Print($"Background image '{imageName}' not found.");
        }
    }
}
