using Godot;

public partial class BackgroundImage : TextureRect
{
    public void Set(string imageName)
    {
        if (imageName == "none")
        {
            Texture = null;
        }
        else if (AssetManager.GetBackground(imageName, out var image))
        {
            Texture = image;
        }
        else
        {
            GD.Print($"Background image '{imageName}' not found.");
        }
    }
}
