using Godot;
using System;

public partial class EquipmentSlot : SlotBase 
{
	[Export] public Items.ItemType ItemType;	// What type of item this slot can hold
	
	private PauseMenu _pauseMenu;
	
	public override void _Ready() {
		base._Ready();
		var parent = GetParent();
		
		while (parent != null && parent is not CanvasLayer)
			parent = parent.GetParent();

		_pauseMenu = parent as PauseMenu;
	}

	public override void RemoveItem(int c = 1) {
		EquipmentChanged(Item, false);
		base.RemoveItem(c);
	}
	
	public override bool _CanDropData(Vector2 atPosition, Variant data) {
		var item = (Items.Item)data;
		return item.Type == ItemType;
	}

	public override void AcceptDraggedItem(Items.Item rItem, int count = 1) {
		CopyItem(rItem);
		SetCount(count);
	}
	
	public override void _DropData(Vector2 atPosition, Variant data) { 
		var draggedSlot = (SlotBase)data;

		switch (Item, draggedSlot) {
			case (null, var rSlot):
				AcceptDraggedItem(rSlot.GetItem());
				rSlot.RemoveItem(1);
				EquipmentChanged(Item, true);
				break;
			case var (item, rSlot):
				var tmpSlot = new SlotBase();
				tmpSlot.CopyItem(item);
				
				AcceptDraggedItem(rSlot.GetItem());
				rSlot.AcceptDraggedItem(tmpSlot.GetItem());
				
				EquipmentChanged(tmpSlot.GetItem(), false);
				EquipmentChanged(item, true);
				break;
		}

		Dragging = false;
	}


	// This function is disgusting, but it took me a long time to get it exactly right,
	// covering all the edge cases, and I'm scared to refactor it.
	/*private void MoveItemToSlot(Slot slot, Func<Slot, bool> condition) {
		if (!condition(slot))
			return;

		switch (Type) {
			case SlotType.Inventory when slot._item is not null:
			{
				if (slot._item == _item && _item.StackSize > 1) {
					slot.SetCount(slot._item.Count + _item.Count);
					RemoveItem(1);
					return;
				}

				Slot newSlot;
				if ((newSlot = FindInventorySlot(_pauseMenu.GetNode("%InventoryGrid"), 
					    slot => slot._item is null)) == null)
					return;
			
				slot.EquipmentChanged(slot._item, false);
				newSlot.CopyItem(slot._item);
				slot.RemoveItem(slot._item.Count);
				
				slot.CopyItem(_item);
				slot.SetCount(1);
				RemoveItem(1);
				slot.EquipmentChanged(slot._item, true);
				break;
			}
			case SlotType.Inventory:
				slot.CopyItem(_item);
				slot.SetCount(1);
				RemoveItem(1);
				slot.EquipmentChanged(slot._item, true);
				break;
			case SlotType.Equipment:
			{
				if (slot._item is not null && slot._item == _item) {
					if (_item.StackSize > 1) {
						slot.SetCount(slot._item.Count + _item.Count);
						RemoveItem(_item.Count);
						return;
					}
					
					Slot newSlot;
					if ((newSlot = FindInventorySlot(_pauseMenu.GetNode("%InventoryGrid"), 
					    slot => slot._item is null)) == null)
						return;
					EquipmentChanged(_item, false);
					newSlot.CopyItem(slot._item);
					RemoveItem(slot._item.Count);
					return;
				}
				
				slot.CopyItem(_item);
				slot.SetCount(_item.Count);
				EquipmentChanged(_item, false);
				RemoveItem(_item.Count);
				break;
			}
		}
	}
	
	public override void _GuiInput(InputEvent @event) {
		switch (@event) {
			case InputEventMouseButton {ButtonIndex: MouseButton.Left, DoubleClick: true}:
				if (_item is not null) {
					var parent = _pauseMenu.GetNode("%InventoryGrid");
					SlotBase slot;
					if ((slot = FindSlot(parent, 
							slot => _item is not null || slot._item == _item)) == null)
						return;
					
					MoveItemToSlot(slot, slot => true);
				}
				break;
		}
	}*/
	
	private void EquipmentChanged(Items.Item item, bool equip) {
		_pauseMenu.EmitSignal(PauseMenu.SignalName.EquipmentChangedSignal, item, equip);
	}
}
