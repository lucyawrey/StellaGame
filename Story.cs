using Godot;
using System;
using GodotInk;

public partial class Story : VBoxContainer
{
	[Export]
	private InkStory story;
	
	public override void _Ready()
	{
		Label content = new() { Text = story.ContinueMaximally() };
		AddChild(content);
	}
}
