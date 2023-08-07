using Godot;
using Godot.NativeInterop;
using System;
using System.Security.Authentication.ExtendedProtection;

public partial class SlotHover : Panel {
	public TextureRect Icon;
	public RichTextLabel Name;
	public RichTextLabel Description;
	public RichTextLabel Value;

	public override void _Ready() {
		Icon = GetNode<TextureRect>("Contents/Header/Icon");
		Name = GetNode<RichTextLabel>("Contents/Header/ItemName");
		Description = GetNode<RichTextLabel>("Contents/Description");
		Value = GetNode<RichTextLabel>("Contents/Footer");
	}

	public void SetName(string value) => Name.Text = "[color=green]" + value + "[/color]";
	public void SetDescription(string value) => Description.Text = "[i]" + value + "[/i]";
	public void SetValue(string value) => Value.Text = "[right]Value: [color=yellow]" + value + "[/color][/right]";
	public void SetIcon(Texture2D value) => Icon.Texture = value;
}
