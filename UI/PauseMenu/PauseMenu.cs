using Godot;
using System;
using System.Linq;

public partial class PauseMenu : CanvasLayer {
	/*[Signal] public delegate void EquipmentChangedSignalEventHandler(Items.Item item, bool equip);
	
	private GridContainer _container;
	private PackedScene _slot = ResourceLoader.Load<PackedScene>("res://UI/Inventory/Slot.tscn");
	private Viewport _viewPort;
	private TextureRect _texture;

	private Player _player;
	
	public override void _Ready() {
		_container = GetNode<GridContainer>("%InventoryGrid");
		
		foreach (var node in GetTree().GetNodesInGroup("Players")) {
			if (node is not Player p) continue;
			_player = p;
			//p.PauseMenuSignal += ShowPauseMenu;
		}
	}

	private void ReleaseSubViewport() {
		_viewPort = GetNode<SubViewport>("%CharacterPortrait");
		
		_viewPort.GetChild(0).QueueFree();
	}
	
	private void SetupSubViewport() {
		_viewPort = GetNode<SubViewport>("%CharacterPortrait");
		
		var pDuplicate = _player.Duplicate() as Entity;
		var camera = pDuplicate.GetNode<Node3D>("CameraController");
		var spring = pDuplicate.GetNode<SpringArm3D>("CameraController/SpringArm3D");
		camera.SetPhysicsProcess(false);

		_viewPort.AddChild(pDuplicate);
		camera.Position = Vector3.Zero;
		camera.Rotation = new Vector3(0, _player.Mesh.Rotation.Y, 0);
		spring.SpringLength = 1.6f;
		spring.Position = new Vector3(0.0f, 0.75f, 0.0f);
		spring.Rotation = new Vector3(0.0f, 0.0f, 0.0f);
	}

	private Slot AddSlot() {
		if (_slot.Instantiate() is not Slot slot) return null;
		_container.AddChild(slot);
		slot.Visible = true;
		slot.Type = Slot.SlotType.Inventory;
		slot.ItemType = Items.ItemType.None;

		return slot;
	}
	
	private void ShowPauseMenu(Player p) {
		Visible = true;
		GetTree().Paused = true;

		while (_container.GetChildCount() != 0 && _container.GetChild(0) is { } child) {
			_container.RemoveChild(child);
			child.QueueFree();
		}

		var count = 0;
		foreach (var item in p.GetInventory().GetItems().Where(item => !item.IsWearing)) {
			if (item.StackSize > 1) {
				var slot = AddSlot();
				slot.CopyItem(item);
				count += item.Count;
			}
			else {
				for (var i = 0; i < item.Count; i++) {
					var slot = AddSlot();
					slot.CopyItem(item);
					slot.SetCount(1);
					count++;
				}	
			}

			if (count >= Inventory.MaxSize) break;
		}

		for (int i = count; i < Inventory.MaxSize; i++) {
			if (_slot.Instantiate() is not Slot slot) return;
			_container.AddChild(slot);
			slot.Visible = true;
			slot.Type = Slot.SlotType.Inventory;
			slot.ItemType = Items.ItemType.None;
		}
		
		SetupSubViewport();
	}
	
	private void HidePauseMenu() {
		GetTree().Paused = false;
		Visible = false;
		ReleaseSubViewport();
	}

	public override void _Input(InputEvent @event) {
		switch (@event) {
			case InputEventKey {PhysicalKeycode: Key.Escape, Pressed: true}:
				if (!GetViewport().IsInputHandled())
					HidePauseMenu();
				GetViewport().SetInputAsHandled();
				break;
		}
	}

	private void OnCloseButtonPressed() => HidePauseMenu();*/
}
