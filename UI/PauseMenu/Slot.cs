using Godot;
using System;

public partial class Slot : Panel {
	private TextureRect _icon;
	private Label _count;

	private PackedScene _hover = ResourceLoader.Load<PackedScene>("res://UI/PauseMenu/SlotHover.tscn");
	private SlotHover _hoverPanel;
	
	public string Name;
	public string Description;
	public int Value;
	
	public override void _Ready() {
		_icon = GetNode<TextureRect>("Icon");
		_count = GetNode<Label>("Count");
		_count.Visible = false;
	}
	
	public void SetCount(int c) {
		if (c <= 1) return;
		_count.Text = c.ToString();
		_count.Visible = true;
	}

	public void SetIcon(string icon) {
		_icon.Texture = GD.Load<Texture2D>(icon);
		//_icon.Scale = new Vector2(Size.X / _icon.Texture.GetWidth(), Size.Y / _icon.Texture.GetHeight());
		//_icon.Position = new Vector2(Size.X / 2, Size.Y / 2);
	}

	private void SetupHoverPanel() {
		_hoverPanel?.SetName(Name);
		_hoverPanel?.SetDescription(Description);
		_hoverPanel?.SetValue(Value.ToString());
		_hoverPanel?.SetIcon(_icon.Texture);

		Vector2 pos;
		pos.X = Size.X - 10;
		pos.Y = Size.Y - 10;
		_hoverPanel.Position = pos;
	}
	
	private void OnSlotMouseEntered() {
		if (Name is not null) {
			_hoverPanel = _hover.Instantiate() as SlotHover;
			AddChild(_hoverPanel);
			SetupHoverPanel();
		}
	}

	private void OnSlotMouseExited() {
		if (Name is not null) {
			_hoverPanel?.QueueFree();
		}
	}
}
