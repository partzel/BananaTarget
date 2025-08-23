using Godot;
using System;

public partial class Abyss : Area3D
{
    [Export] public BananaPool BananaPool;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body.IsInGroup("banana"))
        {
            BananaPool.ReturnBanana((RigidBody3D) body);
        }
    }
}
