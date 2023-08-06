using Godot;
using System;

public partial class PauseMenu : CanvasLayer {
	private GridContainer _container;
	private PackedScene _slot = ResourceLoader.Load<PackedScene>("res://UI/PauseMenu/Slot.tscn");
	
	public override void _Ready() {
		_container = GetNode<GridContainer>("Panel/HFlowContainer/GridContainer");
		
		foreach (var node in GetTree().GetNodesInGroup("Players")) {
			if (node is not Player p) continue;
			p.PauseMenuSignal += ShowPauseMenu;
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
			if (_slot.Instantiate() is not Slot slot) return;
			_container.AddChild(slot);
			slot.SetIcon(item.Icon);
			slot.SetCount(item.Count);
			slot.Name = item.Name;
			slot.Description = item.Description;
			slot.Value = item.Value;
			slot.Visible = true;
		}

		for (int i = p.GetInventory().GetItems().Count; i < Inventory.MaxSize; i++) {
			if (_slot.Instantiate() is not Slot slot) return;
			_container.AddChild(slot);
			slot.Visible = true;
		}
	}
	
	private void HidePauseMenu() {
		GetTree().Paused = false;
		Visible = false;
	}

	public override void _UnhandledKeyInput(InputEvent @event) {
		if (@event is not InputEventKey {Pressed: true, PhysicalKeycode: Key.Escape})
			return;
		
		HidePauseMenu();
		GetViewport().SetInputAsHandled();
	}

	private void OnCloseButtonPressed() => HidePauseMenu();
}
