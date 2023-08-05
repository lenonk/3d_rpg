using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	public override void _Ready() {
		//Player.PauseMenuEventHandler += ActivatePauseMenu;
	}

	private void ActivatePauseMenu(Player p) {
		GetTree().Paused = !GetTree().Paused;
		Visible = !Visible;
	}
}
