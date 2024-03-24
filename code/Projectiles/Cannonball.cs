using System;

namespace Pirate.Projectiles;

[Title( "Projectiles - Cannonball" ), Category( "Pirates" )]
public sealed class Cannonball : Component, Component.ICollisionListener
{
	[Property] public required Rigidbody Rigidbody { get; set; }
	[Property] public required GameObject ShipCollisionEmitter { get; set; }
	[Property] public float FalloffModifier { get; set; } = 1f;

	public void OnCollisionStart( Collision other )
	{
		if ( other.Other.GameObject.Tags.Has( "ship" ) )
		{
			HandleShipCollision( other.Other );
			return;
		}

		if ( other.Other.GameObject.Tags.Has( "solid" ) )
		{
			HandleSolidCollision( other.Other );
			return;
		}

		if ( other.Other.GameObject.Tags.HasAny( "water" ) )
			HandleWaterCollision( other.Other );
	}

	private void HandleShipCollision( CollisionSource source )
	{
		ShipCollisionEmitter.Clone( Transform.Position );
		GameObject.Destroy();
	}

	private void HandleSolidCollision( CollisionSource source )
	{
		Log.Info( "omg i hit solid ground" );
		GameObject.Destroy();
	}

	private void HandleWaterCollision( CollisionSource source )
	{
		Log.Info( "omg i hit water" );
		// play a sound and particle
	}

	protected override void OnFixedUpdate()
	{
		Rigidbody.ApplyForce( Scene.PhysicsWorld.Gravity / MathF.Sqrt( FalloffModifier ) * Rigidbody.PhysicsBody.Mass );
	}
}
