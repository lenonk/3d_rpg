using Godot;
using System;

public partial class InventorySlot : SlotBase 
{
	public override void AcceptDraggedItem(Items.Item rItem, int count = 1) {
		if (Item is not null && Item == rItem && Item.StackSize > 1) {
			SetCount(Item.Count + count);
			return;
		}
		
		CopyItem(rItem);
		SetCount(count);
	}
	
	public override async void _DropData(Vector2 atPosition, Variant data) {
		var draggedSlot = (SlotBase)data;
		var dragCount = draggedSlot.GetItem().Count;

		if (Input.IsActionPressed("ui_shift") && draggedSlot.GetItem().Count > 1) {
			await ShowDragDialog();
			dragCount = Math.Max(0, Math.Min(DragNumber, dragCount));
			if (dragCount <= 0) return;
		}

		switch (Item, draggedSlot.GetItem()) {
			case (null, var rItem):
				AcceptDraggedItem(rItem, dragCount);
				draggedSlot.RemoveItem(dragCount);
				break;
			case var (item, rItem):
				var tmpSlot = new SlotBase();
				tmpSlot.CopyItem(item);
				
				AcceptDraggedItem(rItem, dragCount);
				
				draggedSlot.RemoveItem(dragCount);
				//rSlot.AcceptDraggedItem(tmpSlot);	
				break;
		}

		Dragging = false;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data) {
		return data is SlotBase;
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
				if (_item is not null && _item.IsWearable()) {
					if (Type == SlotType.Inventory) {
						var parent = _pauseMenu.GetNode("%EquipmentContainer");
						Slot slot;
						if ((slot = FindEquipmentSlot(parent, _item.Type)) == null)
							return;
						
						MoveItemToSlot(slot, slot => true);
					}
					else {
						var parent = _pauseMenu.GetNode("%InventoryGrid");
						Slot slot;
						if ((slot = FindInventorySlot(parent, 
							    slot => _item is not null && slot._item == _item)) == null)
							return;
						
						MoveItemToSlot(slot, slot => true);
					}
				}
				break;
		}
	}*/
}
