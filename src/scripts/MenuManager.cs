using Godot;

namespace StellaGame;

public partial class MenuManager : Control
{
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
    public StateManager State { get; set; }

    [Export]
    public Control Content { get; set; }

    public void Open()
    {
        Visible = true;
        Content.Visible = false;
        ContinueButton.GrabFocus();
    }

    public override void _Ready()
    {
        ContinueButton.Pressed += () =>
        {
            Visible = false;
            if (!State.GameIsLoaded)
            {
                State.LoadGame();
            }
            else
            {
                Content.Visible = true;
            }
        };
        ContinueButton.MouseEntered += ContinueButton.GrabFocus;
        if (!State.SaveExists())
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
