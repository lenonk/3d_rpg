using Godot;
using System;

public partial class PauseMenu : CanvasLayer {
	private GridContainer _container;
	private PackedScene _slot = ResourceLoader.Load<PackedScene>("res://UI/Inventory/InventorySlot.tscn");
	private Viewport _viewPort;
	private TextureRect _texture;
	
	public override void _Ready() {
		_container = GetNode<GridContainer>("Panel/HFlowContainer/GridContainer");
		
		foreach (var node in GetTree().GetNodesInGroup("Players")) {
			if (node is not Player p) continue;
			p.PauseMenuSignal += ShowPauseMenu;
			_viewPort = GetNode<SubViewport>("SubViewport");
			
			var pDuplicate = p.Duplicate() as Entity;
			var camera = pDuplicate.GetNode<Node3D>("CameraController");
			var spring = pDuplicate.GetNode<SpringArm3D>("CameraController/SpringArm3D");

			camera.SetPhysicsProcess(false);
			_viewPort.AddChild(pDuplicate);
			spring.SpringLength = 1.6f;
			spring.Position = new Vector3(0, 1, 0);
			spring.Rotation = new Vector3(0, 0, 0);
		}
	}

	private void ShowPauseMenu(Player p) {
		Visible = true;
		GetTree().Paused = true;

		while (_container.GetChildCount() != 0 && _container.GetChild(0) is { } child) {
			_container.RemoveChild(child);
			child.QueueFree();
		}

		foreach (Items.Item item in p.GetInventory().GetItems()) {
			if (_slot.Instantiate() is not InventorySlot slot) return;
			_container.AddChild(slot);
			slot.SetItem(item);
			slot.Visible = true;
			slot.Type = InventorySlot.SlotType.Inventory;
			slot.ItemType = Items.ItemType.None;
		}

		for (int i = p.GetInventory().GetItems().Count; i < Inventory.MaxSize; i++) {
			if (_slot.Instantiate() is not InventorySlot slot) return;
			_container.AddChild(slot);
			slot.Visible = true;
			slot.Type = InventorySlot.SlotType.Inventory;
			slot.ItemType = Items.ItemType.None;
		}
	}
	
	private void HidePauseMenu() {
		GetTree().Paused = false;
		Visible = false;
	}

	public override void _UnhandledKeyInput(InputEvent @event) {
		if (@event is not InputEventKey {Pressed: true, PhysicalKeycode: Key.Escape})
			return;
		
		if (!GetViewport().IsInputHandled())
			HidePauseMenu();
		GetViewport().SetInputAsHandled();
	}

	private void OnCloseButtonPressed() => HidePauseMenu();
}
