using Godot;

namespace StellaGame;

public partial class Menu : Control
{
    [Export]
    public Game Game { get; set; }

    [Export]
    public Button ContinueButton { get; set; }

    [Export]
    public Button LoadButton { get; set; }

    [Export]
    public Button ExtrasButton { get; set; }

    [Export]
    public Button SettingsButton { get; set; }

    [Export]
    public Button ExitButton { get; set; }

    [Export]
    public StoryManager Story { get; set; }

    public void Open()
    {
        Visible = true;
        ContinueButton.GrabFocus();
    }

    public override void _Ready()
    {
        ContinueButton.Pressed += () =>
        {
            Visible = false;
            if (!Game.State.GameIsLoaded)
            {
                Game.State.LoadGame();
            }
            else
            {
                Story.Open();
            }
        };
        ContinueButton.MouseEntered += ContinueButton.GrabFocus;
        if (!Game.State.SaveExists())
        {
            ContinueButton.Text = "New Game";
        }

        LoadButton.MouseEntered += LoadButton.GrabFocus;

        ExtrasButton.MouseEntered += ExtrasButton.GrabFocus;

        SettingsButton.MouseEntered += SettingsButton.GrabFocus;

        ExitButton.Pressed += () => GetTree().Quit();
        ExitButton.MouseEntered += ExitButton.GrabFocus;

        ContinueButton.GrabFocus();
    }
}
