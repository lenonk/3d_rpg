using Godot;

public partial class Enemy : Entity {
	[Export] public const float Speed = 3.0f;
	[Export] public const float ChaseRadius = 15.0f;
	[Export] public const float AttackRadius = 1.5f;
	
	private AnimationPlayer _animationPlayer;
	private Area3D _chaseArea;
	private Player _target;
	private Vector3 _direction;
	private Node3D _mesh;
	private int _health = 10;

	private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	
	public override void _Ready() {
		base._Ready();
		
		_chaseArea = CreateArea3D("ChasePlayer", ChaseRadius);
		_chaseArea.Connect("body_entered", new Callable(this, nameof(OnChaseBodyEntered)));
		_chaseArea.Connect("body_exited", new Callable(this, nameof(OnChaseBodyExited)));
		
		_mesh = GetNode<Node3D>("Skeleton3D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		GetNode<AnimationTree>("AnimationTree").Active = false;
	}

	public override void _PhysicsProcess(double delta) {
		var velocity = Velocity;
		
		if (!IsOnFloor())
			velocity.Y -= (float)(_gravity * delta);
		
		if (_target == null) {
			_direction = Vector3.Zero;
			_animationPlayer.Play("Idle");
		}
		else {
			_direction = (_target.Position - Position).Normalized();

			if (Position.DistanceTo(_target.Position) <= AttackRadius) {
				velocity.X = velocity.Z = 0;
				_mesh.Rotation = GetMeshRotationAngle(delta);
				velocity = Vector3.Zero;
				_animationPlayer.Play("Attack(1h)");
			}
			else if (Position.DistanceTo(_target.Position) - _target.Size <= ChaseRadius) {
				_mesh.Rotation = GetMeshRotationAngle(delta);
				velocity.X = _direction.X * Speed;
				velocity.Z = _direction.Z * Speed;
				_animationPlayer.Play("Run");
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}
	
	public override void TakeDamage(int value) {
		if (_health > 0 && (_health -= value) <= 0)
			Die();
	}

	private Vector3 GetMeshRotationAngle(double delta) {
		Vector3 angle = _mesh.Rotation; 
		angle.Y = (float)Mathf.LerpAngle(
			_mesh.Rotation.Y,
			Mathf.Atan2(_direction.X, _direction.Z) - Rotation.Y, 
			delta * 10);

		return angle;
	}
		
	private Area3D CreateArea3D(string name, float size) {
		Area3D area = new Area3D();
		area.Name = name;
		CollisionShape3D cShape = CreateCollisionShape(size);

		area.AddChild(cShape);
		AddChild(area);

		return area;
	}
	
	private CollisionShape3D CreateCollisionShape(float size) {
		var collisionShape = new CollisionShape3D();
		collisionShape.Shape = new SphereShape3D();
		((SphereShape3D)collisionShape.Shape).Radius = size;
		
		return collisionShape;
	}

	private async void Die() {
		SetPhysicsProcess(false);
		_animationPlayer.Stop();
		_animationPlayer.Play("Defeat");
		await ToSignal(_animationPlayer, "animation_finished");
		await ToSignal(GetTree().CreateTimer(3.0f), "timeout");
		// TODO: I want the dead entity to fade out
		QueueFree();
	}

	private void OnChaseBodyEntered(Node3D body) {
		if (body is not Player player) return;
		_target = player;
	}
	
	private void OnChaseBodyExited(Node3D body) {
		if (body is not Player) return;
		_target = null;
	}
}