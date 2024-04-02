namespace Pirate.Ship;

[Title( "Physics - Floating Point" ), Category( "Pirates" )]
public sealed class FloatingPoint : Component
{
	[Property] public required Rigidbody Rigidbody { get; set; }
	[Property] public float DensityModifier { get; set; } = 1f;
	[Property] public float SampleModifier { get; set; } = 1f;
	[Property] public int SamplePoints { get; set; }
	[Property] public bool DrawSamplingDebug { get; set; } = false;
	[Property] public bool UseAverageSamplePosition { get; set; } = true;

	private Vector3[]? RandomPoints { get; set; }

	protected override void OnStart()
	{
		base.OnStart();

		RandomPoints = new Vector3[SamplePoints];

		var bounds = Rigidbody.PhysicsBody.GetBounds();
		var hhBounds = new BBox( bounds.Mins, bounds.Maxs.WithZ( bounds.Maxs.z * SampleModifier ) );
		for ( var i = 0; i < SamplePoints; i++ )
		{
			RandomPoints[i] = Transform.World.PointToLocal( hhBounds.RandomPointInside );
		}
	}

	protected override void OnFixedUpdate()
	{
		if ( RandomPoints is null )
			return;

		var submergedPoints = new List<Vector3>();

		for ( var i = 0; i < RandomPoints.Length; i++ )
		{
			var worldPoint = Transform.World.PointToWorld( RandomPoints[i] );
			var waveHeight = Waves.Instance.GetWaveHeight( worldPoint.x );
			if ( worldPoint.z < waveHeight )
				submergedPoints.Add( worldPoint );
		}

		// Calculate average position of submerged area
		Vector3 averagePosition = 0f;
		foreach ( var point in submergedPoints )
		{
			averagePosition += point;
		}

		averagePosition /= submergedPoints.Count;

		var massSubmerged = (float)submergedPoints.Count / RandomPoints.Length;

		Rigidbody.ApplyForceAt( UseAverageSamplePosition ? averagePosition : Rigidbody.PhysicsBody.MassCenter,
			-Scene.PhysicsWorld.Gravity * Rigidbody.PhysicsBody.Mass * massSubmerged * 10f * (1f / DensityModifier) );
	}

	protected override void DrawGizmos()
	{
		base.DrawGizmos();

		Gizmo.Transform = global::Transform.Zero;

		Gizmo.Draw.LineThickness = 2;
		Gizmo.Draw.Color = Color.Yellow;
		Gizmo.Draw.LineBBox( Rigidbody.PhysicsBody.GetBounds() );

		if ( DrawSamplingDebug )
		{
			Gizmo.Transform = Transform.Local;

			if ( RandomPoints is not null )
			{
				for ( var i = 0; i < RandomPoints.Length; i++ )
				{
					var point = Transform.World.PointToWorld( RandomPoints[i] );
					Gizmo.Draw.Color = point.z < 0 ? Color.Green : Color.Red;
					Gizmo.Draw.LineSphere( RandomPoints[i], 2f );
				}
			}
		}
	}
}
