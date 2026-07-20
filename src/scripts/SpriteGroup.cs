using System.Collections.Generic;
using Godot;

namespace StellaGame;

public partial class SpriteGroup : Control
{
    private Dictionary<string, Vector2> PositionMap = new()
    {
        { "center", new Vector2(0, 0) },
        { "left", new Vector2(-160, 0) },
        { "right", new Vector2(160, 0) },
    };

    private Vector2 PositionToVector(string position)
    {
        var finalVector = new Vector2(0, 0);
        foreach (var rawPositionToken in position.Split(","))
        {
            var positionToken = rawPositionToken.Trim();
            if (PositionMap.TryGetValue(positionToken, out var additiveVector))
            {
                finalVector += additiveVector;
                continue;
            }

            var rawKeyValueTokens = positionToken.Split(":");
            if (rawKeyValueTokens.Length == 2)
            {
                var key = rawKeyValueTokens[0].Trim().ToLower();
                if (float.TryParse(rawKeyValueTokens[1].Trim(), out var value))
                {
                    if (key == "x")
                    {
                        finalVector += new Vector2(value, 0);
                        continue;
                    }
                    else if (key == "y")
                    {
                        finalVector += new Vector2(0, value);
                        continue;
                    }
                }
            }
        }
        return finalVector;
    }

    public void AddSprite(string imageName, string position)
    {
        if (AssetManager.GetSprite(imageName, out var image))
        {
            var sprite = new Sprite2D
            {
                Texture = image,
                Name = imageName,
                Position = PositionToVector(position),
            };
            AddChild(sprite);
        }
        else
        {
            GD.Print($"Sprite '{imageName}' not found.");
        }
    }

    public void RemoveSprite(string imageName)
    {
        foreach (var child in GetChildren())
        {
            if (child.Name == imageName)
            {
                child.QueueFree();
            }
        }
    }

    public void RemoveAllSprites()
    {
        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }
    }
}
