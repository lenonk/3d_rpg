using Godot;
using System;
using System.Threading.Tasks;

public partial class SlotBase : Panel {
	private TextureRect _icon;
	private Label _count;
	private SlotHover _hoverPanel;
    
	private PackedScene _hoverScene = ResourceLoader.Load<PackedScene>("res://UI/Inventory/SlotHover.tscn");
	private PackedScene _dragPreviewScene = ResourceLoader.Load<PackedScene>("res://UI/DragPreview/DragPreview.tscn");
	private PackedScene _dragDialogScene = ResourceLoader.Load<PackedScene>("res://UI/DragDialog/DragDialog.tscn");

	protected bool Dragging;
	protected Items.Item? Item;
	protected int DragNumber = 1;
	
	public override void _Ready() {
		_icon = GetNode<TextureRect>("Icon");
		_count = GetNode<Label>("Count");
		_count.Visible = false;
		Item = null;
	}

	public Items.Item GetItem() => Item;
	public void SetItem(Items.Item item) {
		if (item is null) return;
		
		Item = item;
		SetCount(item.Count);
		_icon.Texture = GD.Load<Texture2D>(item.Icon);
	}
	
	public void CopyItem(Items.Item item) {
		if (item is null) return;
		
		Item = Items.Item.CreateItem(item.Name);
		Item.Count = item.Count;
		_icon.Texture = GD.Load<Texture2D>(item.Icon);
	}
	
	public void SetCount(int c) {
		if (Item is null) return;
		
		Item.Count = Math.Max(0, c);

		if (Item.Count <= 1) {
			_count.Visible = false;
			return;
		}
		
		_count.Text = c.ToString();
		_count.Visible = true;
	}

	private void SetupHoverPanel() {
		if (Item is null || Dragging || _hoverPanel is null) return;
		
		_hoverPanel.SetName(Item.Name);
		_hoverPanel.SetDescription(Item.Description);
		_hoverPanel.SetValue(Item.Value.ToString());
		_hoverPanel.SetIcon(_icon.Texture);

		Vector2 pos;
		pos.X = Size.X - 10;
		pos.Y = Size.Y - 10;
		_hoverPanel.Position = pos;
	}
	
	private void OnSlotMouseEntered() {
		if (Item is null || Dragging) return;

		if (_hoverScene.Instantiate() is SlotHover { } hp) {
			_hoverPanel = hp;
			AddChild(_hoverPanel);
			SetupHoverPanel();
		}
	}

	private void OnSlotMouseExited() {
		if (Item is not null && _hoverPanel is not null) {
			_hoverPanel.QueueFree();
			_hoverPanel = null;
		}
	}

	public virtual void RemoveItem(int c = 1) {
		if (Item is null) return;
		
		SetCount(Item.Count - c);

		if (Item.Count > 0)
			return;

		Item = null;
		_icon.Texture = null;
		_hoverPanel?.QueueFree();
		_hoverPanel = null;
	}

	public virtual void AcceptDraggedItem(Items.Item rItem, int count = 1) {
	}
	
	protected async Task ShowDragDialog() {
		if (_dragDialogScene.Instantiate() is DragDialog { } dialog) {
			AddChild(dialog);
			dialog.Connect(DragDialog.SignalName.DragDialogClose, 
				new Callable(this, nameof(OnDragDialogClose)));
			await ToSignal(dialog, DragDialog.SignalName.DragDialogClose);
		}
	}

	private void CreateDragPreview(Texture2D texture) {
		if (_dragPreviewScene.Instantiate() is DragPreview { } preview) {
			preview.Texture = texture;
			AddChild(preview);
		}
	}
		
	public override Variant _GetDragData(Vector2 atPosition) {
		if (Item is null) return new();

		CreateDragPreview(_icon.Texture);
		Dragging = true;
		return this;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data) {
		return false;
	}

	protected SlotBase FindSlot(Node parent, Predicate<SlotBase> predicate) {
		foreach (var node in parent.GetChildren()) {
			if (node is SlotBase slot && predicate(slot))
				return slot;
		}

		return null;
	}

	private void OnDragDialogClose(int result) => DragNumber = result;
}
