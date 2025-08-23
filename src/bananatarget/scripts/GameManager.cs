using Godot;

public partial class GameManager : Node
{
    [Export] public ToucanPool ToucanPool;
    [Export] public BananaPool BananaPool;
    [Export] public Node3D Monkey;
    [Export] public float RewardPerTarget = 10;

    private float _score = 0;
    private Timer _roundTimer;
    private Vector3 _monkeyStartPosition;


    public override void _Ready()
    {
        _roundTimer = GetNode<Timer>("RoundTimer");
        _roundTimer.Timeout += OnRoundTimeout;
        _monkeyStartPosition = Monkey.GlobalPosition;
        ToucanPool.ToucanScored += OnToucanScored;

        StartRound();
    }

    private void OnToucanScored()
    {
        _score += RewardPerTarget;
    }

    public void StartRound()
    {
        _score = 0;
        ResetEnvironment();
        _roundTimer.Start();
    }

    public void ResetEnvironment()
    {
        // Reset monkey
        Monkey.GlobalPosition = _monkeyStartPosition;
        // maybe reset rotation/velocity if physics
        var rb = Monkey as RigidBody3D;
        if (rb != null) rb.LinearVelocity = rb.AngularVelocity = Vector3.Zero;
    }

    private void OnRoundTimeout()
    {
        GD.Print("Round finished! Score: " + _score);
        StartRound();
    }
}