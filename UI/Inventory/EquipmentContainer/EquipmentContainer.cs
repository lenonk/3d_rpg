using Godot;
using System;

public partial class EquipmentContainer : VFlowContainer {
	private Player _player;

	public override void _Ready() {
		_player = GetTree().GetNodesInGroup("Players")[0] as Player;
	}

	public override void _PhysicsProcess(double delta) {
		foreach (var child in GetChildren()) {
			if (child is not EquipmentSlot slot) return;

			slot.ClearItem();
			Items.Item item;
			if ((item = _player.GetEquipment().At(slot.Index)) == null)
				continue;
			slot.SetItem(item);
		}
	}
}