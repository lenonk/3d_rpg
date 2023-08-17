using Godot;

public partial class SlotBase : Panel {
	[Export] public int Index;

	[Signal] public delegate void EquipmentChangedEventHandler(int index, bool equip, int rIndex = -1);
	
	protected Player Player;
	
	private SlotHover _hoverPanel;
	private TextureRect _icon;
	private Label _count;
	
	private PackedScene _hoverScene = ResourceLoader.Load<PackedScene>("res://UI/Inventory/Slot/SlotHover.tscn");
	private PackedScene _dragPreviewScene = ResourceLoader.Load<PackedScene>("res://UI/DragPreview/DragPreview.tscn");

	protected enum DragSourceType {
		Equipment,
		Inventory,
	}

	protected partial class DragData: Node {
		public int Index;
		public DragSourceType SourceType;
	}
	
	public override void _Ready() {
		_icon = GetNode<TextureRect>("Icon");
    	_count = GetNode<Label>("Count");
		_count.Visible = false;
	
		Player = GetTree().GetNodesInGroup("Players")[0] as Player;
	}

	public void SetItem(Items.Item item) {
		if (item is null) return;

		SetCount(item.Count);
		_icon.Texture = GD.Load<Texture2D>(item.Icon);
	}
	
	public void ClearItem() {
		_count.Text = "0";
		_count.Visible = false;
		_icon.Texture = null;
	}

	public void SetCount(int c) {
		_count.Text = c.ToString();
		_count.Visible = c > 1;
	}

	protected virtual Items.Item GetItem() {
		return null;
	}
	
	private void SetupHoverPanel() {
		if (Input.IsAnythingPressed() || _hoverPanel is null || GetItem() is null) return;

		_hoverPanel.SetName(GetItem().Name);
		_hoverPanel.SetDescription(GetItem().Description);
		_hoverPanel.SetValue(GetItem().Value.ToString());
		_hoverPanel.SetIcon(_icon.Texture);

		Vector2 pos;
		pos.X = Size.X - 10;
		pos.Y = Size.Y - 10;
		_hoverPanel.Position = pos;
	}

	private void OnSlotMouseEntered() {
		if (Input.IsAnythingPressed() || GetItem() is null) return;

		if (_hoverScene.Instantiate() is SlotHover { } hp) {
			_hoverPanel = hp;
			AddChild(_hoverPanel);
			SetupHoverPanel();
		}
	}

	private void OnSlotMouseExited() {
		if (_hoverPanel is not null) {
			_hoverPanel.QueueFree();
			_hoverPanel = null;
		}
	}

	protected void CreateDragPreview() {
		if (_dragPreviewScene.Instantiate() is DragPreview { } preview) {
			preview.Texture = _icon.Texture;
			AddChild(preview);
		}
	}
}
