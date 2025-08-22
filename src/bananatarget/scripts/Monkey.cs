using Godot;

public partial class Monkey : CharacterBody3D
{
    [Export] public float RotationSpeed = 3.0f;   // radians per second
    [Export] public float MoveSpeed = 5.0f;

    private AnimationPlayer _anim;

    public override void _Ready()
    {
        _anim = GetNode<Node3D>("Model3D")
                .GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _PhysicsProcess(double delta)
    {
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
        if (Input.IsActionJustPressed("throw"))
        {
            if (_anim != null)
            {
                _anim.Play("Throw");
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
        else if (!_anim.IsPlaying())
        {
            _anim.Play("Idle");
        }
    }
}
