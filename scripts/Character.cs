using System.Collections.Generic;

public class Character
{
    public string Name { get; set; }
    public string NameColor { get; set; }
    public string TextColor { get; set; }
    public string Font { get; set; }
    public int FontSize { get; set; }
    public float TextSpeed { get; set; }
    public string Prefix { get; set; }
    public string Suffix { get; set; }
    public string TextSound { get; set; }
    public float TextSoundPitch { get; set; }
    public int TextSoundSpacing { get; set; }
    public bool Fullscreen { get; set; }
    public string Theme { get; set; }
    public List<Character> Forms { get; set; }
}