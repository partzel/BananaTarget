using Godot;

public partial class GameManager : Node
{
    [Export] public ToucanPool pool;
    [Export] public float RewardPerTarget = 10;

    private float score = 0;

    public override void _Ready()
    {
        pool.ToucanScored += OnToucanScored;
    }

    private void OnToucanScored()
    {
        score += RewardPerTarget;
        GD.Print($"Score: {score}");
    }
}