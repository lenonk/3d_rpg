using Godot;
using System;

public partial class InventoryContainer : HFlowContainer {
	private Player _player;
	private GridContainer _inventoryGrid;
	
	public override void _Ready() {
		_inventoryGrid = GetNode<GridContainer>("%InventoryGrid");
		
		int idx = 0;
		foreach (var child in _inventoryGrid.GetChildren()) {
			if (child is not InventorySlot slot) return;
			slot.Index = idx++;
		}
		
		_player = GetTree().GetNodesInGroup("Players")[0] as Player;
	}

	public override void _PhysicsProcess(double delta) {
		foreach (var child in _inventoryGrid.GetChildren()) {
			if (child is not InventorySlot slot) return;

			slot.ClearItem();
			
			Items.Item item;
			if ((item = _player.GetInventory().At(slot.Index)) == null || item.IsWearing)
				continue;
			
			slot.SetItem(item);
		}
	}
}
