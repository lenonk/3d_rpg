using Godot;
using System;

public partial class InventoryUI : CanvasLayer 
{
	private Player _player;
	
	public override void _Ready() {
		_player = GetTree().GetNodesInGroup("Players")[0] as Player;
		SetupSubViewport();
	}

	private void SetupSubViewport() {
		var svp = GetNode<SubViewport>("%CharacterPortrait");
		
		var pDuplicate = _player.Duplicate() as Player;
		pDuplicate.ToggleHealthBar(false);
		// This is a hack to keep this instance of the player from falling forever and glitching out
		pDuplicate.BanGravity(); 
		
		var camera = pDuplicate.GetNode<Node3D>("CameraController");
		var spring = pDuplicate.GetNode<SpringArm3D>("CameraController/SpringArm3D");

		svp.AddChild(pDuplicate);
		camera.Position = Vector3.Zero;
		camera.Rotation = new Vector3(0, _player.Mesh.Rotation.Y, 0);
		spring.SpringLength = 3;
		spring.Position = new Vector3(0.0f, 1.0f, 0.0f);
		spring.Rotation = Vector3.Zero;
	}
	
	private void HideInventoryUI() {
		if (!Visible) return;
		Visible = false;
		_player.ProcessMode = ProcessModeEnum.Pausable;
	}

	private void ShowInventoryUI() {
		if (Visible) return;
		Visible = true;
		_player.ProcessMode = ProcessModeEnum.Disabled;
		
	}
	private void OnClosePressed() {
		HideInventoryUI();
	}
	
	public override void _UnhandledKeyInput(InputEvent @event) {
		switch (@event) {
			case InputEventKey {PhysicalKeycode: Key.Escape, Pressed: true}:
				if (!Visible) return;
				HideInventoryUI();
				GetViewport().SetInputAsHandled();
				break;	
			case InputEventKey {PhysicalKeycode: Key.I, Pressed: true}:
				if (Visible) HideInventoryUI();
				else ShowInventoryUI();
				GetViewport().SetInputAsHandled();
				break;
		}
	}
	
	public override void _UnhandledInput(InputEvent @event) {
		switch (@event) {
			case InputEventMouseButton {Pressed: true}:
				if (Visible)
					GetViewport().SetInputAsHandled();
				break;
		}
	}
}
