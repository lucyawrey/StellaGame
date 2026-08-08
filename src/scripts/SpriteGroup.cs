using System.Collections.Generic;
using Godot;

namespace StellaGame;

public partial class SpriteGroup : Control
{
    [Export]
    public StateManager State { get; set; }

    private Dictionary<string, Vector2> PositionMap = new()
    {
        { "center", new Vector2(0, 0) },
        { "left", new Vector2(-190, 0) },
        { "right", new Vector2(190, 0) },
    };

    public void Load()
    {
        RemoveAllSprites();
        foreach (var entry in State.ActiveSlot.CurrentSpriteGroup)
        {
            AddSprite(entry.Key, entry.Value, false);
        }
    }

    private Vector2 PositionToVector(string position)
    {
        var finalVector = new Vector2(0, 0);
        if (position == null)
        {
            return finalVector;
        }
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

    public void AddSprite(string imageName, string position = null, bool save = true)
    {
        if (AssetLibrary.GetSprite(imageName, out var image))
        {
            if (save)
            {
                State.ActiveSlot.CurrentSpriteGroup.Add(imageName, position);
            }

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
