using Godot;
using System;

public partial class Slot : Panel {
	private Sprite2D _icon;
	private Label _count;

	public override void _Ready() {
		_icon = GetNode<Sprite2D>("Icon");
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
		_icon.Scale = new Vector2(Size.X / _icon.Texture.GetWidth(), Size.Y / _icon.Texture.GetHeight());
		_icon.Position = new Vector2(Size.X / 2, Size.Y / 2);
	}
}
