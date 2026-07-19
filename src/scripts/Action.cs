namespace StellaGame;

public abstract class StoryAction
{
    public class Show : StoryAction
    {
        public required string ImageName { get; set; }
        public required string Position { get; set; }
    }

    public class Hide : StoryAction
    {
        public required string ImageName { get; set; }
    }

    public class Clear : StoryAction;

    public class Background : StoryAction
    {
        public required string ImageName { get; set; }
    }

    public class PlayMusic : StoryAction
    {
        public required string SongName { get; set; }
    }

    public class StopMusic : StoryAction;

    public class PlaySound : StoryAction
    {
        public required string SoundName { get; set; }
    }

    public class Auto : StoryAction
    {
        public required int Count { get; set; }
    };

    public class Delay : StoryAction
    {
        public required float Time { get; set; }
    }

    public class AddItem : StoryAction
    {
        public required string ItemName { get; set; }
        public required int Quantity { get; set; }
    }

    public class RemoveItem : StoryAction
    {
        public required string ItemName { get; set; }
        public required int Quantity { get; set; }
    }
}
