using Godot;
using System;

public partial class InventoryUI : CanvasLayer 
{
	private Player _player;
	
	public override void _Ready() {
		_player = GetTree().GetNodesInGroup("Players")[0] as Player;
	}

	private void SetupSubViewport() {
		var svp = GetNode<SubViewport>("%CharacterPortrait");
		
		var pDuplicate = _player.Duplicate() as Player;
		var camera = pDuplicate.GetNode<Node3D>("CameraController");
		var spring = pDuplicate.GetNode<SpringArm3D>("CameraController/SpringArm3D");
		camera.SetPhysicsProcess(false);

		pDuplicate.ToggleHealthBar(false);
		svp.AddChild(pDuplicate);
		camera.Position = Vector3.Zero;
		camera.Rotation = new Vector3(0, _player.Mesh.Rotation.Y, 0);
		spring.SpringLength = 1.85f;
		spring.Position = new Vector3(-0.615f, 1.0f, 0.0f);
		spring.Rotation = new Vector3(0.0f, Mathf.DegToRad(13.0f), 0.0f);
	}
	
	private void ReleaseSubViewport() {
		var svp = GetNode<SubViewport>("%CharacterPortrait");
		svp.GetChild(0).QueueFree();
	}
	
	private void HideInventoryUI() {
		if (!Visible) return;
		Visible = false;
		ReleaseSubViewport();
		GetTree().Paused = false;
	}

	private void ToggleInventoryUI() {
		GetTree().Paused = !GetTree().Paused;
		if (!Visible) SetupSubViewport();
		else ReleaseSubViewport();
		Visible = !Visible;
	}

	private void OnClosePressed() {
		HideInventoryUI();
		GetViewport().SetInputAsHandled();
	}
	
	public override void _Input(InputEvent @event) {
		switch (@event) {
			case InputEventKey {PhysicalKeycode: Key.Escape, Pressed: true}:
				if (!Visible) return;
				HideInventoryUI();
				GetViewport().SetInputAsHandled();
				break;	
			case InputEventKey {PhysicalKeycode: Key.I, Pressed: true}:
				ToggleInventoryUI();
				GetViewport().SetInputAsHandled();
				break;
		}
	}
}
