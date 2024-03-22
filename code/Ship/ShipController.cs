using System;
using Pirate.World;

namespace Pirate.Ship;

[Title( "Ship - Controller" ), Category( "Pirates" )]
public sealed class ShipController : Component
{
	[Property] public float ShipSpeed { get; set; } = 20f;
	[Property] public float TurningSpeed { get; set; } = 3f;
	[Property] public required Rigidbody Rigidbody { get; set; }
	[Property] public required Wind Wind { get; set; }

	public bool AnchorDropped { get; set; }

	protected override void OnUpdate()
	{
		if ( Input.Pressed( "toggle_anchor" ) )
		{
			ToggleAnchor();
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
		var force = wishSpeed + Wind.WindForce;

		var angle = Transform.Rotation.Forward.Normal.Angle( Wind.WindDirection );
		force *= (90f / angle).Clamp( 0.8f, 2f );

		if ( AnchorDropped )
			return;

		var rot = new Angles( pitch, Transform.Rotation.Yaw() + Input.AnalogMove.y * TurningSpeed, 0f ).ToRotation();
		Rigidbody.ApplyForce( force * Rigidbody.PhysicsBody.Mass );
		Rigidbody.PhysicsBody.SmoothRotate( rot, 1f / TurningSpeed,
			Time.Delta / 12f );
	}

	private Vector3 GetWishSpeed()
	{
		return Transform.Rotation.Forward.Normal * MathF.Abs( Input.AnalogMove.x ) * ShipSpeed;
	}

	protected override void DrawGizmos()
	{
		base.DrawGizmos();

		Gizmo.Transform = global::Transform.Zero;
		var up = Vector3.Up * 128f;
		Gizmo.Draw.Arrow( Transform.Position + up, Transform.Position + Transform.Rotation.Forward * 80f + up, 100f,
			10f );

		Gizmo.Draw.Color = Color.Cyan;
		Gizmo.Draw.Arrow( Transform.Position + up, Transform.Position + new Vector3( Wind.WindDirection ) + up,
			100f,
			10f );
	}
}
