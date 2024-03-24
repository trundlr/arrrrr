using Pirate.Extensions;

namespace Pirate.Ship;

public class GunController : Component
{
	[Property] public List<GameObject> GunSources { get; set; } = new();
	[Property] public required GameObject ProjectilePrefab { get; set; }
	[Property] public float Force { get; set; } = 400f;

	public void Fire()
	{
		GunSources.Shuffle();

		foreach ( var gunSource in GunSources )
		{
			var delay = Game.Random.Int( 40, 160 );
			FireAfter( gunSource.Transform.World, delay );
		}
	}

	private async void FireAfter( Transform source, int ms )
	{
		await GameTask.Delay( ms );

		var sourcePos = source.Position;
		var fireRotation = source.Rotation;

		var projectile = ProjectilePrefab.Clone( sourcePos );
		if ( projectile is null )
			return;

		if ( !projectile.Components.TryGet( out Rigidbody rigidbody ) )
			return;

		rigidbody.ApplyImpulse( fireRotation.Forward * Force * rigidbody.PhysicsBody.Mass );
	}

	protected override void DrawGizmos()
	{
		base.DrawGizmos();

		Gizmo.Transform = global::Transform.Zero;

		foreach ( var gun in GunSources )
		{
			Gizmo.Draw.Arrow( gun.Transform.Position, gun.Transform.Position + gun.Transform.Rotation.Forward * 20f );
		}
	}
}
