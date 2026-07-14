using Godot;
using System.Collections.Generic;

public partial class BackgroundImage : TextureRect
{
    private static Dictionary<string, Texture2D> backgroundLibrary = new()
    {
        { "cafe", GD.Load<Texture2D>("res://assets/backgrounds/cafe.png") },
        { "bedroom", GD.Load<Texture2D>("res://assets/backgrounds/bedroom.png") },
    };

    public static bool GetBackground(string imageName, out Texture2D image)
    {
        return backgroundLibrary.TryGetValue(imageName, out image);
    }

    public void Set(string imageName)
    {
        if (imageName == "none")
        {
            Texture = null;
        }
        else if (GetBackground(imageName, out var image))
        {
            Texture = image;
        }
        else
        {
            GD.Print($"Image not found: {imageName}");
        }
    }
}
