namespace Pirate.World;

[Title( "World - Sounds" ), Category( "Pirates" )]
public sealed class WorldSounds : Component
{
	[Property] public required SoundEvent Ambience { get; set; }
	private SoundHandle? _ambientHandle { get; set; }

	protected override void OnStart()
	{
		_ambientHandle = Sound.Play( Ambience );
	}
}
