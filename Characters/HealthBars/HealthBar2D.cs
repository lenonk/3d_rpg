using Godot;
using System;

public partial class HealthBar2D : TextureProgressBar
{
	private Entity _owner;
	private CompressedTexture2D greenBar;
	private CompressedTexture2D yellowBar;
	private CompressedTexture2D redBar;
	
	public override void _Ready() {	
		_owner = FindOwner();
		greenBar = GD.Load<CompressedTexture2D>("res://Assets/Textures/ProgressBars/green_bar.png");
		yellowBar = GD.Load<CompressedTexture2D>("res://Assets/Textures/ProgressBars/yellow_bar.png");
		redBar = GD.Load<CompressedTexture2D>("res://Assets/Textures/ProgressBars/red_bar.png");
	}

	public override void _PhysicsProcess(double delta) {
		if (_owner == null) return;

		MaxValue = _owner.MaxHealth;
		Value = _owner.Health;
		
		TextureProgress = greenBar;
		if (Value < 0.75f * MaxValue)
			TextureProgress = yellowBar;
		if (Value < 0.45f * MaxValue)
			TextureProgress = redBar;
	}

	private Entity FindOwner() {
		var parent = GetParent();
        
		while (parent != null && parent is not Entity)
			parent = parent.GetParent();

		return parent as Entity;
	}
}
