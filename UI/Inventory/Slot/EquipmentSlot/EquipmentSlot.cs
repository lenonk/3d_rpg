using Godot;

public partial class EquipmentSlot : SlotBase {
	[Export] private Items.ItemType SlotType;

	private int _dragNumber = 1;
	
	public override void _DropData(Vector2 atPosition, Variant data) {
		var dd = (DragData)data;

		if (dd.SourceType != DragSourceType.Inventory)
			return;

		if (Player.GetEquipment().At(Index) != null) {
			EmitSignal(SlotBase.SignalName.EquipmentChanged, Index, false, dd.Index);
		}
		
		EmitSignal(SlotBase.SignalName.EquipmentChanged, dd.Index, true, -1);
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data) {
		var dd = (DragData)data;

		if (dd.SourceType == DragSourceType.Inventory) {
			return Player.GetInventory().At(dd.Index).Type == SlotType;
		}

		return Player.GetEquipment().At(dd.Index).Type == SlotType;
	}
	
	public override Variant _GetDragData(Vector2 atPosition) {
		if (Player.GetEquipment().At(Index) is null) return new();

		CreateDragPreview();
		var dd = new DragData();
		dd.Index = Index;
		dd.SourceType = DragSourceType.Equipment;
		
		return dd;
	}
	
	public override void _GuiInput(InputEvent @event) {
		switch (@event) {
			case InputEventMouseButton {ButtonIndex: MouseButton.Left, DoubleClick: true}:
				var clickedItem = Player.GetEquipment().At(Index);
				if (clickedItem is null) return;
				
				EmitSignal(SlotBase.SignalName.EquipmentChanged, Index, false, -1);
				break;
		}
	}
	
	protected override Items.Item GetItem() => Player.GetEquipment().At(Index);
	private void OnDragDialogClose(int result) => _dragNumber = result;
}
