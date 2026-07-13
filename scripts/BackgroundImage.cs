using Godot;
using System.Collections.Generic;

public partial class BackgroundImage : TextureRect
{
    private Dictionary<string, Texture2D> backgroundLibrary = new()
    {
        { "cafe", GD.Load<Texture2D>("res://assets/backgrounds/cafe.png") },
    };

    public void Set(string imageName)
    {
        if (imageName == "none")
        {
            Texture = null;
        }
        else if (backgroundLibrary.TryGetValue(imageName, out var image))
        {
            Texture = image;
        }
        else
        {
            GD.Print($"Image not found: {imageName}");
        }
    }
}
