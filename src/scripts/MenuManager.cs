using Godot;

namespace StellaGame;

public partial class MenuManager : Control
{
    [Export]
    private Button continueButton;

    [Export]
    private Button loadButton;

    [Export]
    private Button extrasButton;

    [Export]
    private Button settingsButton;

    [Export]
    private Button exitButton;

    [Export]
    private StateManager state;

    [Export]
    private Control content;

    public void Open()
    {
        Visible = true;
        content.Visible = false;
        continueButton.GrabFocus();
    }

    public override void _Ready()
    {
        continueButton.Pressed += () =>
        {
            Visible = false;
            if (!state.GameIsLoaded)
            {
                state.LoadGame();
            }
            else
            {
                content.Visible = true;
            }
        };
        continueButton.MouseEntered += continueButton.GrabFocus;
        if (!state.SaveExists())
        {
            continueButton.Text = "New Game";
        }

        loadButton.MouseEntered += loadButton.GrabFocus;

        extrasButton.MouseEntered += extrasButton.GrabFocus;

        settingsButton.MouseEntered += settingsButton.GrabFocus;

        exitButton.Pressed += () => GetTree().Quit();
        exitButton.MouseEntered += exitButton.GrabFocus;

        continueButton.GrabFocus();
    }
}
