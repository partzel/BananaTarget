using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class BananaPool : Node
{
	[Export] public PackedScene BananaScene;
	[Export] public int PoolSize = 9;

	public Queue<RigidBody3D> InactivePool = new();
	public Queue<RigidBody3D> ActivePool = new();

	public override void _Ready()
	{
		// Pre-instantiate bananas
		for (int i = 0; i < PoolSize; i++)
		{
			var banana = BananaScene.Instantiate<RigidBody3D>();
			banana.Visible = false; // hide until used
			banana.Sleeping = true; // don’t simulate physics yet
			AddChild(banana);
			InactivePool.Enqueue(banana);
		}
	}

	public RigidBody3D GetBanana()
	{
		if (!InactivePool.Any())
			return null;

		var banana = InactivePool.Dequeue();
		ActivePool.Enqueue(banana);

		banana.Sleeping = false;
		banana.SetProcess(true);
		banana.Visible = true;
		return banana;
	}

	public void ReturnBanana()
	{
		if (IsActivePoolEmpty())
			return;

		RigidBody3D banana = ActivePool.Dequeue();
		banana.Visible = false;
		banana.SetProcess(false);
		banana.Sleeping = true;
		InactivePool.Enqueue(banana);
	}

	// For GDscript interop
	public RigidBody3D GetLatestBanana()
	{
		if (IsActivePoolEmpty())
			return null;

		return ActivePool.Peek();
	}


	public bool IsActivePoolEmpty()
	{
		return !ActivePool.Any();
	}
}
