using Godot;
using System;

public partial class InventorySlot : Panel {
	private Items.Item? _item;
	private TextureRect _icon;
	private Label _count;
	private SlotHover _hoverPanel;

	private PackedScene _hover = ResourceLoader.Load<PackedScene>("res://UI/Inventory/SlotHover.tscn");
	
	public override void _Ready() {
		_icon = GetNode<TextureRect>("Icon");
		_count = GetNode<Label>("Count");
		_count.Visible = false;
		_item = null;
	}

	public void SetItem(Items.Item item) {
		_item = item;
		SetCount(item.Count);
		_icon.Texture = GD.Load<Texture2D>(item.Icon);
	}
	
	public void SetCount(int c) {
		if (c <= 1) return;
		_count.Text = c.ToString();
		_count.Visible = true;
	}

	private void SetupHoverPanel() {
		if (_item is null || _hoverPanel is null) return;
		
		_hoverPanel.SetName(_item?.Name);
		_hoverPanel.SetDescription(_item?.Description);
		_hoverPanel.SetValue(_item?.Value.ToString());
		_hoverPanel.SetIcon(_icon.Texture);

		Vector2 pos;
		pos.X = Size.X - 10;
		pos.Y = Size.Y - 10;
		_hoverPanel.Position = pos;
	}
	
	private void OnSlotMouseEntered() {
		if (_item is null) return;

		if (_hover.Instantiate() is SlotHover hp) {
			_hoverPanel = hp;
			AddChild(_hoverPanel);
			SetupHoverPanel();
		}
	}

	private void OnSlotMouseExited() {
		if (_item is not null) {
			_hoverPanel?.QueueFree();
		}
	}
	
	public override void _GuiInput(InputEvent @event) {
	}
}
