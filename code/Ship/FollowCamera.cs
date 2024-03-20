namespace Pirate.Ship;

[Title( "Follow Camera" ), Category( "Pirates" )]
public sealed class FollowCamera : Component
{
	[Property] public GameObject? Target { get; set; }
	public float Distance { get; set; } = 1024f;

	protected override void OnUpdate()
	{
		if ( Target is null )
			return;

		var cam = GameObject;
		var targetPos = Target.Transform.Position + cam.Transform.Rotation.Backward * Distance;
		cam.Transform.Position = cam.Transform.Position.LerpTo( targetPos, Time.Delta * 5f );

		Distance -= Input.MouseWheel.y * 32f;
		Distance = Distance.Clamp( 128f, 4096f );
	}
}
