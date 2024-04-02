using System;
using Pirate.World;

namespace Pirate.Ship;

[Title( "Ship - Controller" ), Category( "Pirates" )]
public sealed class ShipController : Component
{
	[Property] public float ShipSpeed { get; set; } = 20f;
	[Property] public float TurningSpeed { get; set; } = 3f;
	[Property] public bool PlayerControlled { get; set; } = false;
	[Property] public ModelRenderer? Model { get; set; }
	[Property] public required Rigidbody Rigidbody { get; set; }
	[Property] public required GunController Guns { get; set; }
	[Property] public GameObject? FishingRodPrefab { get; set; }

	public bool AnchorDropped { get; set; }
	public bool Fishing { get; set; }

	private GameObject? FishingRod { get; set; }
	private Vector3 FishingMountLeft { get; set; }
	private Vector3 FishingMountRight { get; set; }
	private Vector3 FishingMountLeftWorld => Transform.World.PointToWorld( FishingMountLeft );
	private Vector3 FishingMountRightWorld => Transform.World.PointToWorld( FishingMountRight );

	protected override void OnStart()
	{
		if ( Model is null )
			return;

		FishingRod = FishingRodPrefab?.Clone();
		if ( FishingRod is not null )
		{
			FishingRod.Parent = GameObject;
			FishingRod.Enabled = false;
		}


		var fishingAttachmentL = Model.Model.GetAttachment( "fishing_mount_L" );
		if ( fishingAttachmentL is not null )
		{
			FishingMountLeft = fishingAttachmentL.Value.Position;
		}

		var fishingAttachmentR = Model.Model.GetAttachment( "fishing_mount_R" );
		if ( fishingAttachmentR is not null )
		{
			FishingMountRight = fishingAttachmentR.Value.Position;
		}
	}

	protected override void OnUpdate()
	{
		if ( Input.Pressed( "toggle_anchor" ) )
		{
			ToggleAnchor();
		}

		if ( Input.Pressed( "destroy_ship" ) )
		{
			var mast = GameObject.Children.FirstOrDefault( c => c.Name == "Mast" );
			if ( mast is null )
				return;
			mast.Parent = Scene;
			mast.Transform.Position = Transform.Position;
			mast.Components.Get<Rigidbody>( true ).Enabled = true;
			mast.Components.Get<FloatingPoint>( true ).Enabled = true;
		}

		if ( Fishing )
		{
		}
	}

	private void ToggleAnchor()
	{
		AnchorDropped = !AnchorDropped;
	}

	protected override void OnFixedUpdate()
	{
		var pitch = (MathF.Sin( Time.Now ) - 1f) * 3f;

		var wishSpeed = GetWishSpeed();
		var force = wishSpeed + Wind.Instance.WindForce;

		var angle = Transform.Rotation.Forward.Normal.Angle( Wind.Instance.WindDirection );
		force *= (90f / angle).Clamp( 0.8f, 1.5f );

		if ( AnchorDropped )
		{
			force = 0f;
		}

		if ( PlayerControlled )
		{
			if ( Input.Pressed( "fire" ) )
			{
				Guns.Fire();
			}
		}

		var rot = PlayerControlled
			? new Angles( pitch, Transform.Rotation.Yaw() + Input.AnalogMove.y * TurningSpeed, 0f ).ToRotation()
			: new Angles( pitch, Transform.Rotation.Yaw(), 0f ).ToRotation();
		Rigidbody.ApplyForceAt( Rigidbody.PhysicsBody.MassCenter, force * Rigidbody.PhysicsBody.Mass );
		Rigidbody.PhysicsBody.SmoothRotate( rot, 1f / TurningSpeed,
			Time.Delta / 12f );
	}

	public void Hit( Vector3 position, Vector3 velocity, float mass )
	{
		Rigidbody.PhysicsBody.ApplyImpulseAt( position, velocity * mass );
	}

	private Vector3 GetWishSpeed()
	{
		if ( !PlayerControlled )
			return 0f;
		return Transform.Rotation.Forward.Normal * MathF.Abs( Input.AnalogMove.x ) * ShipSpeed;
	}

	public bool CanFish( FishingSpot fishingSpot )
	{
		return fishingSpot.Transform.Position.Distance( Transform.Position ) < 250f;
	}

	public void StartFishing( FishingSpot spot )
	{
		Fishing = true;
		DeployFishingApparatus( spot );
	}

	public void StopFishing()
	{
		Fishing = false;
		HideFishingApparatus();
	}

	private void DeployFishingApparatus( FishingSpot spot )
	{
		if ( FishingRod is null )
			return;

		FishingRod.Enabled = true;

		if ( GetShipFishingSide( spot.Transform.Position ) is ShipSide.Left )
		{
			FishingRod.Transform.Position = FishingMountLeftWorld;
			FishingRod.Transform.LocalRotation = new Angles( 0f, 0f, -75f ).ToRotation();
		}
		else
		{
			FishingRod.Transform.Position = FishingMountRightWorld;
			FishingRod.Transform.LocalRotation = new Angles( 0f, 180f, -75f ).ToRotation();
		}
	}

	private void HideFishingApparatus()
	{
		if ( FishingRod is null )
			return;

		FishingRod.Enabled = false;
	}

	private ShipSide GetShipFishingSide( Vector3 target )
	{
		return FishingMountLeftWorld.Distance( target ) < FishingMountRightWorld.Distance( target )
			? ShipSide.Left
			: ShipSide.Right;
	}

	protected override void DrawGizmos()
	{
		base.DrawGizmos();

		Gizmo.Transform = global::Transform.Zero;
		var up = Vector3.Up * 128f;
		Gizmo.Draw.Arrow( Transform.Position + up, Transform.Position + Transform.Rotation.Forward * 80f + up, 100f,
			10f );

		Gizmo.Draw.Color = Color.Cyan;
		Gizmo.Draw.Arrow( Transform.Position + up, Transform.Position + new Vector3( Wind.Instance.WindDirection ) + up,
			100f,
			10f );
	}

	private enum ShipSide
	{
		Left,
		Right
	}
}
