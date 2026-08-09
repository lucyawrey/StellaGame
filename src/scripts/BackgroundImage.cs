using Godot;

namespace StellaGame;

public partial class BackgroundImage : TextureRect
{
    [Export]
    public Game Game { get; set; }

    public void Load()
    {
        Set("none", false);
        Set(Game.State.ActiveSlot.CurrentBackgroundImage, false);
    }

    public void Set(string imageName, bool save = true)
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
                Game.State.ActiveSlot.CurrentBackgroundImage = imageName;
            }
        }
        else
        {
            GD.Print($"Background image '{imageName}' not found.");
        }
    }
}
