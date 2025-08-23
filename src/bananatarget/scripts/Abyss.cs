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
            GD.Print("Banana fell into abyss → returning to pool");
            BananaPool.ReturnBanana((RigidBody3D) body);
        }
    }
}
