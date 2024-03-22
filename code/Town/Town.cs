namespace Pirate.Town;

public sealed class Town : Component, Component.ITriggerListener
{
	private const float DefaultVisitRadius = 128f;

	[Property] public string TownName { get; set; } = "Town";
	[Property] public int BasePopulation { get; set; }
	[Property] public int FishTradePrice { get; set; }
	[Property] public float VisitRadius { get; set; } = DefaultVisitRadius;

	public int CurrentPopulation { get; set; }

	public void OnTriggerEnter( Collider other )
	{
		Log.Info( other );
	}

	public void OnTriggerExit( Collider other )
	{
		Log.Info( other + " exit" );
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
