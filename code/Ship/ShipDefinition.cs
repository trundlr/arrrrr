using System;

namespace Pirate.Ship;

[GameResource( "Ship Definition", "ship", "Definition for a ship", Icon = "sailing" )]
public sealed class ShipDefinition : GameResource
{
	[Category( "Meta" )] public string Name { get; set; } = "Ship";
	[Category( "Meta" )] public string Description { get; set; } = "Ship description";

	[Category( "Physics" )] public float MovementSpeed { get; set; } = 80f;
	[Category( "Physics" )] public float TurningSpeed { get; set; } = 3f;
	[Category( "Physics" )] public float LinearDamping { get; set; } = 0.65f;
	[Category( "Physics" )] public float AngularDamping { get; set; } = 0.30f;
	[Category( "Physics" )] public float DensityModifier { get; set; } = 1f;
	[Category( "Physics" )] public float SampleModifier { get; set; } = 0.50f;

	[Category( "Rendering" )] public string HullPath { get; set; } = "";
	[Category( "Rendering" )] public Model? Hull { get; set; }

	[Category( "Construction" )] public List<ShipAttachment> Attachments { get; set; } = new();

	[Category( "Guns" )] public float GunPower { get; set; } = 400f;

	public static List<ShipDefinition> All => ResourceLibrary.GetAll<ShipDefinition>().ToList();

	public static ShipDefinition? GetByName( string name )
	{
		return All.FirstOrDefault( s => s.Name.Equals( name, StringComparison.InvariantCultureIgnoreCase ) );
	}
}
