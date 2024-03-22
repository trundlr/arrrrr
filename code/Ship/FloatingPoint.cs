namespace Pirate.Ship;

public sealed class FloatingPoint : Component
{
	[Property] public required Rigidbody Rigidbody { get; set; }
	[Property] public required Waves Waves { get; set; }

	[Property] public float DepthBeforeSubmerged { get; set; } = 1f;
	[Property] public float DisplacementAmount { get; set; } = 3f;
	[Property] public int FloatSources { get; set; } = 1;

	[Property] public int SamplePoints { get; set; }

	private Vector3[]? RandomPoints { get; set; }

	protected override void OnStart()
	{
		base.OnStart();

		RandomPoints = new Vector3[SamplePoints];

		var bounds = Rigidbody.PhysicsBody.GetBounds();
		for ( var i = 0; i < SamplePoints; i++ )
		{
			RandomPoints[i] = Transform.World.PointToLocal( bounds.RandomPointInside );
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
			var waveHeight = Waves.GetWaveHeight( worldPoint.x );
			if ( worldPoint.z < waveHeight )
				submergedPoints.Add( worldPoint );
		}

		// Calculate average position of submerged area
		// Vector3 averagePosition = 0f;
		// foreach ( var point in submergedPoints )
		// {
		// 	averagePosition += point;
		// }
		//
		// averagePosition /= submergedPoints.Count;

		var massSubmerged = (float)submergedPoints.Count / RandomPoints.Length;

		Rigidbody.ApplyForce( // Rigidbody.PhysicsBody.MassCenter,
			-Scene.PhysicsWorld.Gravity * Rigidbody.PhysicsBody.Mass * massSubmerged * 10f );
	}

	protected override void DrawGizmos()
	{
		base.DrawGizmos();

		Gizmo.Transform = global::Transform.Zero;

		Gizmo.Draw.LineThickness = 2;
		Gizmo.Draw.Color = Color.Yellow;
		Gizmo.Draw.LineBBox( Rigidbody.PhysicsBody.GetBounds() );

		// Gizmo.Transform = Transform.Local;
		//
		// if ( RandomPoints is not null )
		// {
		// 	for ( var i = 0; i < RandomPoints.Length; i++ )
		// 	{
		// 		var point = Transform.World.PointToWorld( RandomPoints[i] );
		// 		Gizmo.Draw.Color = point.z < 0 ? Color.Green : Color.Red;
		// 		Gizmo.Draw.LineSphere( RandomPoints[i], 4f );
		// 	}
		// }
	}
}

// Log.Info( bounds );
// Apply gravity

// if ( Transform.Position.z < 0f )
// 	Rigidbody.ApplyForceAt( Transform.Position,
// 		-Scene.PhysicsWorld.Gravity * Rigidbody.PhysicsBody.Mass / FloatSources * 10f );

// Rigidbody.ApplyForceAt( Transform.Position,
// 	Scene.PhysicsWorld.Gravity * Rigidbody.PhysicsBody.Mass / FloatSources );
// var waveHeight = Waves.GetWaveHeight( Transform.Position.x );
// if ( Transform.Position.z < waveHeight )
// {
// 	var displacementMult =
// 		((waveHeight - Transform.Position.z) / DepthBeforeSubmerged).Clamp( 0f, 1f ) * DisplacementAmount;
// 	Log.Info( displacementMult );
// 	Rigidbody.ApplyForceAt( Transform.Position,
// 		new Vector3( 0f, 0f, -Scene.PhysicsWorld.Gravity.z * displacementMult ) * Rigidbody.PhysicsBody.Mass /
// 		FloatSources );
// }
