using System;
using Godot;

public partial class Monkey : CharacterBody3D
{
	[Export] public float RotationSpeed = 3.0f;   // radians per second
	[Export] public float MoveSpeed = 5.0f;
	[Export] public float ThrowForce = 10.0f;
	[Export] public float ThrowCooldown = 2f;

	private Node3D _bananaSpawnLocation;
	private AnimationPlayer _anim;
	private BananaPool _bananaPool;
	private TextureProgressBar _cooldownBar;

	private bool _isThrowing;
	private double _lastThrowTime;
	private float _cooldownRemaining;

	public override void _Ready()
	{
		_bananaSpawnLocation = GetNode<Node3D>("BananaSpawnLocation");
		_anim = GetNode<Node3D>("Model3D")
			   .GetNode<AnimationPlayer>("AnimationPlayer");
		_bananaPool = GetNode<BananaPool>("BananaPool");
		_cooldownBar = GetNode<CanvasLayer>("CooldownUI")
					  .GetNode<TextureProgressBar>("ProgressBar");

		_isThrowing = false;

		_anim.Play("Idle");

		_cooldownRemaining = 0;
		_cooldownBar.Visible = false;
	}

	public override void _Process(double delta)
	{
		if (_cooldownRemaining > 0)
		{
			_cooldownRemaining -= (float)delta;
			_cooldownBar.Value = _cooldownRemaining / ThrowCooldown * 100;
			if (_cooldownRemaining <= 0)
			{
				_cooldownBar.Visible = false;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isThrowing)
			return;

		HandleMovement((float)delta);
		HandleAnimations();
	}

	private void HandleMovement(float delta)
	{
		Vector3 velocity = Vector3.Zero;

		// Rotate left/right (around Y axis)
		if (Input.IsActionPressed("ui_left"))
			RotateY(-RotationSpeed * delta);
		if (Input.IsActionPressed("ui_right"))
			RotateY(RotationSpeed * delta);

		// Move forward
		if (Input.IsActionPressed("ui_up"))
		{
			// Forward is -Z in Godot 3D
			velocity = -Transform.Basis.Z * MoveSpeed;
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void HandleAnimations()
	{
		bool canThrow = _cooldownRemaining <= 0 && !_isThrowing;

		if (Input.IsActionJustPressed("throw") && canThrow)
		{
			if (_anim is not null)
			{
				_isThrowing = true;
				_anim.Play("Throw");

				_cooldownRemaining = ThrowCooldown;
				_cooldownBar.Visible = true;
				_cooldownBar.Value = 100;
			}
		}
		else if (Input.IsActionPressed("ui_up"))
		{
			_anim.Play("Walk");
		}
		else if (Input.IsActionJustReleased("ui_up"))
		{
			_anim.Play("Idle");
		}
	}

	private void OnThrowRelease()
	{
		var banana = _bananaPool.GetBanana();

		if (banana is null) return;

		banana.GlobalTransform = _bananaSpawnLocation.GlobalTransform;
		banana.LinearVelocity = -Transform.Basis.Z * ThrowForce;

		_lastThrowTime = Time.GetTicksMsec()/1000.0;
	}

	private void OnThrowFinished()
	{
		_isThrowing = false;
		_anim.Play("Idle");
	}

}
