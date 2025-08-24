using System;
using Godot;

public partial class GameManager : Node
{
    [Export] public ToucanPool ToucanPool;
    [Export] public BananaPool BananaPool;
    [Export] public Monkey Monkey;
    [Export] public float RewardPerTarget = 10;

    private float _totalReward = 0;
    private Timer _roundTimer;
    private Vector3 _monkeyStartPosition;


    public override void _Ready()
    {
        _roundTimer = GetNode<Timer>("RoundTimer");
        _roundTimer.Timeout += OnRoundTimeout;
        _monkeyStartPosition = Monkey.GlobalPosition;
        ToucanPool.ToucanScored += OnToucanScored;
        Monkey.Reward += OnReward;

        StartRound();
    }

    private void OnReward(float reward)
    {
        _totalReward += reward;
    }

    private void OnToucanScored()
    {
        _totalReward += RewardPerTarget;
    }

    public void StartRound()
    {
        _totalReward = 0;
        ResetEnvironment();
        _roundTimer.Start();
    }

    public void ResetEnvironment()
    {
        Monkey.GlobalPosition = _monkeyStartPosition;
        Monkey.RotateY((float)GD.RandRange(-Math.PI, Math.PI));
    }

    private void OnRoundTimeout()
    {
        GD.Print("Round finished! Score: " + _totalReward);
        StartRound();
    }
}