using System;
using Pirate.Ship;

namespace Pirate.Projectiles;

[Title( "Projectiles - Cannonball" ), Category( "Pirates" )]
public sealed class Cannonball : Component, Component.ICollisionListener
{
	private const float DestructionTime = 3f;

	[Property] public required Rigidbody Rigidbody { get; set; }
	[Property] public required GameObject ShipCollisionEmitter { get; set; }
	[Property] public required ModelRenderer Model { get; set; }
	[Property] public float FalloffModifier { get; set; } = 1f;

	private TimeUntil TimeUntilDestruction { get; set; }
	private bool QueuedForDestruction { get; set; }

	public void OnCollisionStart( Collision other )
	{
		if ( other.Other.GameObject.Tags.Has( "ship" ) )
		{
			HandleShipCollision( other );
			return;
		}

		if ( other.Other.GameObject.Tags.Has( "solid" ) )
		{
			HandleSolidCollision( other );
			return;
		}

		if ( other.Other.GameObject.Tags.HasAny( "water" ) )
			HandleWaterCollision( other );
	}

	private void HandleShipCollision( Collision source )
	{
		if ( !source.Other.GameObject.Components.TryGet( out ShipController ship ) )
			return;

		Sound.Play( "cannon_ship_impact", source.Contact.Point );
		ShipCollisionEmitter.Clone( Transform.Position );
		ship.Hit( source.Contact.Point, source.Contact.Speed, Rigidbody.PhysicsBody.Mass * 10f );
		GameObject.Destroy();
	}

	private void HandleSolidCollision( Collision source )
	{
		Log.Info( "omg i hit solid ground" );
		if ( source.Contact.Point.z > 0f )
		{
			TimeUntilDestruction = DestructionTime;
			QueuedForDestruction = true;
			return;
		}

		GameObject.Destroy();
	}

	private void HandleWaterCollision( Collision source )
	{
		Sound.Play( "water_splash", source.Contact.Point );
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( TimeUntilDestruction > 0f )
		{
			var tintAlpha = TimeUntilDestruction / DestructionTime;
			Model.Tint = Model.Tint.WithAlpha( tintAlpha );
		}

		if ( QueuedForDestruction && TimeUntilDestruction < 0f )
			GameObject.Destroy();
	}

	protected override void OnFixedUpdate()
	{
		Rigidbody.ApplyForce( Scene.PhysicsWorld.Gravity / MathF.Sqrt( FalloffModifier ) * Rigidbody.PhysicsBody.Mass );
	}
}
