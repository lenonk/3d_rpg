using Godot;
using System;
using System.Threading.Tasks;

public partial class InventorySlot : SlotBase {
	private int _dragNumber = 1;
	
	private PackedScene _dragDialogScene = ResourceLoader.Load<PackedScene>("res://UI/DragDialog/DragDialog.tscn");
	
	private async Task ShowDragDialog() {
		if (_dragDialogScene.Instantiate() is DragDialog { } dialog) {
			AddChild(dialog);
			dialog.Connect(DragDialog.SignalName.DragDialogClose,
				new Callable(this, nameof(OnDragDialogClose)));
			await ToSignal(dialog, DragDialog.SignalName.DragDialogClose);
		}
	}
	
	public override async void _DropData(Vector2 atPosition, Variant data) {
		var dd = (DragData)data;

		_dragNumber = 0;
		if (Input.IsActionPressed("ui_shift") && Player.GetInventory().At(dd.Index).IsStackable()) {
			await ShowDragDialog();
			_dragNumber = Math.Clamp(_dragNumber, 1, Player.GetInventory().At(dd.Index).Count);
		}	
		
		if (dd.SourceType == DragSourceType.Inventory)
			Player.GetInventory().MoveItem(dd.Index, Index, _dragNumber);
		else {
			EmitSignal(SlotBase.SignalName.EquipmentChanged, dd.Index, false, Index);
		}
		Dragging = false;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data) {
		return true;
	}
	
	public override Variant _GetDragData(Vector2 atPosition) {
		if (Player.GetInventory().At(Index) is null) return new();

		CreateDragPreview();
		Dragging = true;
		
		var dd = new DragData();
		dd.Index = Index;
		dd.SourceType = DragSourceType.Inventory;
		
		return dd;
	}

	protected override Items.Item GetItem() => Player.GetInventory().At(Index);
	private void OnDragDialogClose(int result) => _dragNumber = result;
}
