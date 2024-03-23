namespace Pirate.World;

[Title( "World - Wind" ), Category( "Pirates" )]
public sealed class Wind : Component
{
	public Vector3 WindDirection => WindForce.Normal;
	[Property] public Vector3 WindForce { get; set; } = new(2f, 0f);
}
