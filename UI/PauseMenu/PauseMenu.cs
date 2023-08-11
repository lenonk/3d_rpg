using Godot;
using System;

public partial class PauseMenu : CanvasLayer {
	[Signal] public delegate void EquipmentChangedSignalEventHandler(Items.Item item);
	
	private GridContainer _container;
	private PackedScene _slot = ResourceLoader.Load<PackedScene>("res://UI/Inventory/InventorySlot.tscn");
	private Viewport _viewPort;
	private TextureRect _texture;

	private Player _player;
	
	public override void _Ready() {
		_container = GetNode<GridContainer>("%InventoryGrid");
		
		foreach (var node in GetTree().GetNodesInGroup("Players")) {
			if (node is not Player p) continue;
			_player = p;
			p.PauseMenuSignal += ShowPauseMenu;
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
		camera.Rotation = Vector3.Zero;
		spring.SpringLength = 1.6f;
		spring.Position = new Vector3(0.0f, 0.75f, 0.0f);
		spring.Rotation = new Vector3(0.0f, 0.0f, 0.0f);
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
			if (!item.IsWearing) {
				_container.AddChild(slot);
				slot.SetItem(item);
				slot.Visible = true;
				slot.Type = InventorySlot.SlotType.Inventory;
				slot.ItemType = Items.ItemType.None;
			}
		}

		for (int i = p.GetInventory().GetItems().Count; i < Inventory.MaxSize; i++) {
			if (_slot.Instantiate() is not InventorySlot slot) return;
			_container.AddChild(slot);
			slot.Visible = true;
			slot.Type = InventorySlot.SlotType.Inventory;
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

	public void OnEquipmentChanged(Items.Item item, bool equip) => 
		EmitSignal(SignalName.EquipmentChangedSignal, item, equip);
	
	private void OnCloseButtonPressed() => HidePauseMenu();
}
