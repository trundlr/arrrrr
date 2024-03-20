namespace Pirate.World;

[Title( "Wind" ), Category( "Pirates" )]
public sealed class Wind : Component
{
	public Vector3 WindDirection => WindForce.Normal;
	public Vector3 WindForce { get; set; }

	protected override void OnStart()
	{
		base.OnStart();

		WindForce = new Vector3( 2f, 0f );
	}
}
