namespace Pirate.World;

[Title( "World - Fishing Spot" ), Category( "Pirates" )]
public sealed class FishingSpot : Component
{
	[Property] public required ParticleRingEmitter Emitter { get; set; }
	[Property] public float Radius { get; set; } = 100f;

	protected override void OnStart()
	{
		Emitter.Radius = Radius;
	}


	protected override void OnUpdate()
	{
	}

	protected override void DrawGizmos()
	{
		base.DrawGizmos();

		Gizmo.Transform = global::Transform.Zero;

		Gizmo.Draw.LineCylinder( Transform.Position, Transform.Position + Vector3.Up * 4f, Radius, Radius, 8 );
	}
}
