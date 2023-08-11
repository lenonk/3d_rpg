using Godot;
using System;

public partial class HealthBar3D : Sprite3D {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Texture = GetNode<SubViewport>("SubViewport").GetTexture();
	}
}
