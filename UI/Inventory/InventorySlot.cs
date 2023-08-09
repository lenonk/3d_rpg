using Godot;
using System;
using System.Threading.Tasks;

public partial class InventorySlot : Panel {
	private Items.Item? _item;
	private TextureRect _icon;
	private Label _count;
	private SlotHover _hoverPanel;
	private bool _dragging = false;
	private int _dragNumber = 1;
    
	private PackedScene _hoverScene = ResourceLoader.Load<PackedScene>("res://UI/Inventory/SlotHover.tscn");
	private PackedScene _dragPreviewScene = ResourceLoader.Load<PackedScene>("res://UI/DragPreview/DragPreview.tscn");
	private PackedScene _dragDialogScene = ResourceLoader.Load<PackedScene>("res://UI/DragDialog/DragDialog.tscn");
	
	private partial class DragData : Control {
		public InventorySlot Slot;
		public int Count;
	}
    	
	public override void _Ready() {
		_icon = GetNode<TextureRect>("Icon");
		_count = GetNode<Label>("Count");
		_count.Visible = false;
		_item = null;
	}

	public void SetItem(Items.Item item) {
		if (item is null) return;
		
		_item = item;
		SetCount(item.Count);
		_icon.Texture = GD.Load<Texture2D>(item.Icon);
	}
	
	private void NewItem(Items.Item item) {
		if (item is null) return;
		
		_item = Items.Item.CreateItem(item.Name);
		_icon.Texture = GD.Load<Texture2D>(item.Icon);
	}
	
	private void SetCount(int c) {
		if (_item is null) return;
		
		_item.Count = c;

		if (c <= 1) {
			_count.Visible = false;
			return;
		}
		
		_count.Text = c.ToString();
		_count.Visible = true;
	}

	private void SetupHoverPanel() {
		if (_item is null || _dragging || _hoverPanel is null) return;
		
		_hoverPanel.SetName(_item.Name);
		_hoverPanel.SetDescription(_item.Description);
		_hoverPanel.SetValue(_item.Value.ToString());
		_hoverPanel.SetIcon(_icon.Texture);

		Vector2 pos;
		pos.X = Size.X - 10;
		pos.Y = Size.Y - 10;
		_hoverPanel.Position = pos;
	}
	
	private void OnSlotMouseEntered() {
		if (_item is null || _dragging) return;

		if (_hoverScene.Instantiate() is SlotHover hp) {
			_hoverPanel = hp;
			AddChild(_hoverPanel);
			SetupHoverPanel();
		}
	}

	private void OnSlotMouseExited() {
		if (_item is not null && _hoverPanel is not null) {
			_hoverPanel.QueueFree();
			_hoverPanel = null;
		}
	}

	private void RemoveItem(int c = 1) {
		if (_item is null) return;
		
		SetCount(_item.Count - c);		
		if (_item.Count <= 0) {
			_item = null;
			_icon.Texture = null;
		}
	}

	private void ReplaceSlot(DragData dragData) {
		if (_item is null) return;
		
		Items.Item tmpItem = Items.CreateItem(_item.Name);
		tmpItem.Count = _item.Count;
		SetItem(dragData.Slot._item);

		dragData.Slot.SetItem(tmpItem);	
	}
	
	private async Task ShowDragDialog() {
		if (_dragDialogScene.Instantiate() is DragDialog dialog) {
			AddChild(dialog);
			dialog.Connect(DragDialog.SignalName.DragDialogClose, 
				new Callable(this, nameof(OnDragDialogClose)));
			await ToSignal(dialog, DragDialog.SignalName.DragDialogClose);
		}
	}
	
	public override async void _DropData(Vector2 atPosition, Variant data) {
		var dragData = (DragData)data;

		if (Input.IsActionPressed("ui_shift")) {
			await ShowDragDialog();
			dragData.Count = Math.Max(0, Math.Min(_dragNumber, dragData.Count));
		}

		switch (_item, dragData.Slot._item) {
			case (null, var item):
				NewItem(item);
				SetCount(dragData.Count);
				break;
			case var (item1, item2) when item1 == item2:
				SetCount(item1.Count + dragData.Count);
				break;
			case var (item1, item2):
				ReplaceSlot(dragData);
				return;
		}

		dragData.Slot.RemoveItem(dragData.Count);
		_dragging = false;
	}

	public override Variant _GetDragData(Vector2 atPosition) {
		if (_item is null) return new();

		DragData dragData = new();
		dragData.Slot = this;
		dragData.Count = _item.Count;
	
		if (_dragPreviewScene.Instantiate() is DragPreview preview) {
			preview.Texture = _icon.Texture;
			AddChild(preview);
		}

		_dragging = true;
		return dragData;
	}
	
	public override bool _CanDropData(Vector2 atPosition, Variant data) => true;
	private void OnDragDialogClose(int result) => _dragNumber = result;
}
