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

    public override void _Ready()
    {
        continueButton.Pressed += () =>
        {
            Visible = false;
            content.Visible = true;
            if (!state.GameIsLoaded)
            {
                state.LoadGame();
            }
        };
        exitButton.Pressed += () => GetTree().Quit();
    }
}
