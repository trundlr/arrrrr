namespace Pirate.Ship;

[Title( "Ship - Builder Singleton" ), Category( "Pirates" )]
public sealed class ShipBuilder : Component
{
	public ShipBuilder()
	{
		Instance = this;
	}

	public static ShipBuilder Instance { get; set; }
	public ShipController? PlayerShip { get; set; }
	[Property] public required GameObject ShipPrefab { get; set; }

	protected override void OnStart()
	{
		base.OnStart();

		var ship = FromResource( "sloop" );
		if ( ship is null )
		{
			Log.Warning( "Couldn't create ship. WTF!" );
			return;
		}

		PlayerShip = ship.Components.Get<ShipController>();
		var camera = Scene.Camera.Components.Get<FollowCamera>();
		camera.Target = ship;
	}

	public GameObject? FromResource( string name )
	{
		var ship = ShipPrefab.Clone();
		if ( ship is null )
			return null;

		ship.BreakFromPrefab();

		var def = ShipDefinition.GetByName( name );
		if ( def is null )
			return null;

		ship.Name = def.Name;

		var renderer = ship.Components.Get<ModelRenderer>();
		Log.Info( $"def.Hull {def.Hull}" );
		Log.Info( renderer );
		renderer.Model = def.Hull;

		var rigidbody = ship.Components.Get<Rigidbody>();
		rigidbody.LinearDamping = def.LinearDamping;
		rigidbody.AngularDamping = def.AngularDamping;

		var controller = ship.Components.Get<ShipController>();
		controller.ShipSpeed = def.MovementSpeed;
		controller.TurningSpeed = def.TurningSpeed;
		controller.Guns.Force = def.GunPower;

		var floating = ship.Components.Get<FloatingPoint>();
		floating.DensityModifier = def.DensityModifier;
		floating.SampleModifier = def.SampleModifier;

		foreach ( var c in Components.GetAll() )
		{
			c.Enabled = true;
		}

		foreach ( var attachment in def.Attachments )
		{
			var go = new GameObject();

			if ( attachment.Model is not null )
			{
				var aRenderer = go.Components.Create<ModelRenderer>();
				aRenderer.Model = attachment.Model;

				if ( attachment.ModelAttachmentName is not null )
				{
					var att = renderer.Model?.GetAttachment( attachment.ModelAttachmentName );
					if ( att.HasValue )
					{
						go.Transform.Position = att.Value.Position;
						go.Transform.Rotation = att.Value.Rotation;
					}
					else
					{
						go.Transform.Position = attachment.Position;
						go.Transform.Rotation = attachment.Rotation;
					}
				}
			}

			if ( !attachment.ShowByDefault )
			{
				go.Enabled = false;
			}

			go.SetParent( ship );
		}

		return ship;
	}
}
