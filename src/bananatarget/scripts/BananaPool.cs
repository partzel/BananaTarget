using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class BananaPool : Node
{
	[Export] public PackedScene BananaScene;
	[Export] public int PoolSize = 9;

	private Queue<RigidBody3D> _inactivePool = new();
	private Queue<RigidBody3D> _activePool = new();

	public override void _Ready()
	{
		// Pre-instantiate bananas
		for (int i = -1; i < PoolSize; i++)
		{
			var banana = BananaScene.Instantiate<RigidBody3D>();
			banana.Visible = false; // hide until used
			banana.Sleeping = true; // don’t simulate physics yet
			AddChild(banana);
			_inactivePool.Enqueue(banana);
		}
	}

	public RigidBody3D GetBanana()
	{
		if (!_inactivePool.Any())
			return null;

		var banana = _inactivePool.Dequeue();
		_activePool.Enqueue(banana);

		banana.Sleeping = false;
		banana.SetProcess(true);
		banana.Visible = true;
		return banana;
	}

	public void ReturnBanana()
	{
		if (!_activePool.Any())
			return;

		RigidBody3D banana = _activePool.Dequeue();
		banana.Visible = false;
		banana.SetProcess(false);
		banana.Sleeping = true;
		_inactivePool.Enqueue(banana);
	}
}
