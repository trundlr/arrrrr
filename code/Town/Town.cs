using Pirate.UI;

namespace Pirate.Town;

public sealed class Town : Component, Component.ITriggerListener
{
	private const float DefaultVisitRadius = 128f;

	[Property] public string TownName { get; set; } = "Town";
	[Property] public int BasePopulation { get; set; }
	[Property] public int FishTradePrice { get; set; }
	[Property] public float VisitRadius { get; set; } = DefaultVisitRadius;
	[Property] public required TownInfo TownInfo { get; set; }

	public int CurrentPopulation { get; set; }

	public void OnTriggerEnter( Collider other )
	{
		if ( other.GameObject.Root.Name == "Root (Pivot)" )
			TownInfo.ShowStats = true;
	}

	public void OnTriggerExit( Collider other )
	{
		if ( other.GameObject.Root.Name == "Root (Pivot)" )
			TownInfo.ShowStats = false;
	}

	protected override void OnStart()
	{
		CurrentPopulation = BasePopulation;
	}

	protected override void OnUpdate()
	{
	}

	protected override void DrawGizmos()
	{
		base.DrawGizmos();

		Gizmo.Transform = global::Transform.Zero;

		Gizmo.Draw.LineCylinder( Transform.Position, Transform.Position + Vector3.Up * 4f, VisitRadius,
			VisitRadius - 4f,
			8 );
	}
}
