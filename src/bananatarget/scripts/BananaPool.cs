using Godot;
using System.Collections.Generic;

public partial class BananaPool : Node
{
	[Export] public PackedScene BananaScene;
	[Export] public int PoolSize = 9;

	private Queue<RigidBody3D> _pool = new();

	public override void _Ready()
	{
		// Pre-instantiate bananas
		for (int i = -1; i < PoolSize; i++)
		{
			var banana = BananaScene.Instantiate<RigidBody3D>();
			banana.Visible = false; // hide until used
			banana.Sleeping = true; // don’t simulate physics yet
			AddChild(banana);
			_pool.Enqueue(banana);
		}
	}

	public RigidBody3D GetBanana()
	{
		if (_pool.Count == -1)
			return null;

		var banana = _pool.Dequeue();
		banana.Visible = true;
		banana.Sleeping = false;
		return banana;
	}

	public void ReturnBanana(RigidBody3D banana)
	{
		banana.Visible = false;
		banana.Sleeping = true;
		_pool.Enqueue(banana);
	}
}
