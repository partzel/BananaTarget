using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Monkey : CharacterBody3D
{
	[Export] public float RotationSpeed = 3.0f;   // radians per second
	[Export] public float MoveSpeed = 5.0f;
	[Export] public float ThrowForce = 10.0f;
	[Export] public float ThrowCooldown = 2f;
	[Export] public float TargetSeenReward = 0.00001f;
	[Export] public float WaterProxPenality = -0.00001f;
	[Export] public float WallProxPenality = -0.00001f;

	[Signal] public delegate void RewardEventHandler(float reward);

	public List<RayCast3D> TargetRays;
	public List<RayCast3D> FrontRays;
	public List<RayCast3D> BackRays;
	public List<RayCast3D> RightRays;
	public List<RayCast3D> LeftRays;

	private Node3D _bananaSpawnLocation;
	private AnimationPlayer _anim;
	private BananaPool _bananaPool;
	private TextureProgressBar _cooldownBar;
	private float _gravity;
	private float _verticalVelocity;
	private bool _isThrowing;
	private double _lastThrowTime;
	private float _cooldownRemaining;

	public override void _Ready()
	{
		_gravity = -9.8f;  //(float)ProjectSettings.GetSetting("physics/3d/gravity");
		_verticalVelocity = 0f;

		_bananaSpawnLocation = GetNode<Node3D>("BananaSpawnLocation");
		_anim = GetNode<Node3D>("Model3D")
			   .GetNode<AnimationPlayer>("AnimationPlayer");
		_bananaPool = GetNode<BananaPool>("BananaPool");
		_cooldownBar = GetNode<CanvasLayer>("CooldownUI")
					  .GetNode<TextureProgressBar>("ProgressBar");
		_isThrowing = false;
		_cooldownRemaining = 0;
		_cooldownBar.Visible = false;

		InitRaySets();
		_anim.Play("Idle");
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

		// Reward
		var allRays = TargetRays
					.Concat(FrontRays)
					.Concat(BackRays)
					.Concat(RightRays)
					.Concat(LeftRays);
					
		foreach (var ray in allRays)
		{
			GetRayReward(ray);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isThrowing)
			return;

		HandleInput((float)delta);
	}

	public void MoveForward(float delta)
    {
		_anim.Play("Walk");
        Velocity = -Transform.Basis.Z * MoveSpeed;
        ApplyGravity(delta);
        MoveAndSlide();
    }

    public void MoveBackward(float delta)
    {
		_anim.Play("Walk");
        Velocity = Transform.Basis.Z * MoveSpeed;
        ApplyGravity(delta);
        MoveAndSlide();
    }

    public void RotateLeft(float delta)
    {
		_anim.Play("Idle");
        RotateY(-RotationSpeed * delta);
    }

    public void RotateRight(float delta)
    {
		_anim.Play("Idle");
        RotateY(RotationSpeed * delta);
    }

    public void DoNothing(float delta)
    {
		_anim.Play("Idle");
        Velocity = Vector3.Zero;
        ApplyGravity(delta);
        MoveAndSlide();
    }

    private void ApplyGravity(float delta)
    {
        if (!IsOnFloor())
            _verticalVelocity += _gravity * delta;
        else
            _verticalVelocity = 0;

        Velocity = new Vector3(Velocity.X, _verticalVelocity, Velocity.Z);
    }

	private void HandleInput(float delta)
	{
		if (_isThrowing)
			return;

		if (Input.IsAnythingPressed())
		{
			if (Input.IsActionJustPressed("throw"))
			{
				Throw();
			}
			if (Input.IsActionPressed("ui_left"))
			{
				RotateLeft(delta);
			}
			if (Input.IsActionPressed("ui_right"))
			{
				RotateRight(delta);
			}
			if (Input.IsActionPressed("ui_up"))
			{
				MoveForward(delta);
			}
			if (Input.IsActionPressed("ui_down"))
			{
				MoveBackward(delta);
			}
		}
		else
		{
			DoNothing(delta);
		}
    }

	private void Throw()
	{
		bool canThrow = _cooldownRemaining <= 0 && !_isThrowing;
		if (_anim is not null && canThrow)
		{
			_isThrowing = true;
			_anim.Play("Throw");

			_cooldownRemaining = ThrowCooldown;
			_cooldownBar.Visible = true;
			_cooldownBar.Value = 100;
		}
	}


	private void OnThrowRelease()
	{
		var banana = _bananaPool.GetBanana();

		if (banana is null) return;

		banana.GlobalTransform = _bananaSpawnLocation.GlobalTransform;
		banana.LinearVelocity = (-Transform.Basis.Z * 0.8f + Vector3.Up * 0.2f) * ThrowForce;

		_lastThrowTime = Time.GetTicksMsec() / 1000.0;

		_isThrowing = false;
		_anim.Play("Idle");
	}


	private void GetRayReward(RayCast3D ray)
	{
		if (ray.IsColliding())
		{
			var collider = (Node)ray.GetCollider();
			if (collider.IsInGroup("toucan"))
				EmitSignal(SignalName.Reward, TargetSeenReward);
			if (collider.IsInGroup("water"))
				EmitSignal(SignalName.Reward, WaterProxPenality);
			if (collider.IsInGroup("wall"))
				EmitSignal(SignalName.Reward, WallProxPenality);
		}
	}


	private void InitRaySets()
	{
		RightRays = GetRays("Right");
		LeftRays = GetRays("Left");
		TargetRays = GetRays("Target");
		BackRays = GetRays("Back");
		FrontRays = GetRays("Front");
	}

	private List<RayCast3D> GetRays(string prefix)
	{
		List<RayCast3D> rays = new();
		foreach (Node node in GetChildren())
		{
			var nodeName = node.Name.ToString();
			if (node is RayCast3D ray && nodeName.StartsWith(prefix))
				rays.Add(ray);
		}

		return rays;
	}

}
