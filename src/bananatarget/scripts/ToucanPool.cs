using Godot;
using System.Collections.Generic;

public partial class ToucanPool : Node3D
{
    [Export] public BananaPool BananaPool;
    [Export] public PackedScene ToucanScene;
    [Export] public int PoolSize = 5;
    [Export] public Vector3 SpawnAreaMin = new Vector3(-5, 2, -5);
    [Export] public Vector3 SpawnAreaMax = new Vector3(5, 5, 5);
    [Signal] public delegate void ToucanScoredEventHandler();

    private Queue<Toucan> available = new Queue<Toucan>();
    private uint _toucanCollisionLayer = 3;
    private uint _bananaCollisionLayer = 2;
    public override void _Ready()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            var toucan = ToucanScene.Instantiate<Toucan>();
            toucan.Visible = false;
            toucan.SetProcess(false);

            toucan.ToucanHit += OnToucanHit;

            available.Enqueue(toucan);
            AddChild(toucan);
        }

        SpawnToucan();
    }

    public void SpawnToucan()
    {
        if (available.Count == 0) return;

        var toucan = available.Dequeue();

        var offset = new Vector3(
            (float)GD.RandRange(SpawnAreaMin.X, SpawnAreaMax.X),
            (float)GD.RandRange(SpawnAreaMin.Y, SpawnAreaMax.Y),
            (float)GD.RandRange(SpawnAreaMin.Z, SpawnAreaMax.Z)
        );

        toucan.GlobalPosition = GlobalPosition + offset;

        toucan.CollisionLayer = _toucanCollisionLayer;
        toucan.CollisionMask = _bananaCollisionLayer;
        toucan.Visible = true;
        toucan.AnimationPlayer.Play("Idle");
        toucan.SetProcess(true);
    }

    private void OnToucanHit(Toucan toucan, RigidBody3D banana)
    {
        EmitSignal(SignalName.ToucanScored);

        BananaPool.ReturnBanana(banana);
        ReturnToucan(toucan);
        SpawnToucan();
    }

    private void ReturnToucan(Toucan toucan)
    {
        toucan.CollisionLayer = 0;
        toucan.CollisionMask = 0;
        toucan.AnimationPlayer.Play("Exit");
        available.Enqueue(toucan);
    }
}