using Godot;

public partial class Toucan : Area3D
{
	[Signal] public delegate void ToucanHitEventHandler(Toucan toucan, RigidBody3D banana);
	public AnimationPlayer AnimationPlayer;

	public override void _Ready()
	{
		AnimationPlayer = GetNode<Node3D>("Model3D")
		.GetNode<AnimationPlayer>("AnimationPlayer");
		BodyEntered += OnAreaEntered;
	}

	public void HideToucan()
	{
		Visible = false;
	}

	private void OnAreaEntered(Node3D body)
	{
		if (body.IsInGroup("banana"))
		{
			GD.Print("I was hit!");
			EmitSignal(SignalName.ToucanHit, this, body);
		}
	}
}
