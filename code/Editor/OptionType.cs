using System;
using System.Collections.Generic;
using System.Linq;
using Editor;
using Sandbox;

namespace QuickSwitcher;

[Flags]
public enum OptionType
{
	Asset,
	Action,

	All
}

public static class OptionTypeExtension
{
	public static Color GetColor( this OptionType type )
	{
		return type switch
		{
			OptionType.Asset => Theme.White,
			OptionType.Action => Theme.Primary,
			_ => Color.White
		};
	}
}

public record class Option( OptionType Type, string Name, string ActionText, Pixmap Icon );

public record class AssetOption( OptionType Type, string Name, string ActionText, Pixmap Icon, Asset asset )
	: Option( Type, Name, ActionText, Icon );

public record class ActionOption(
	OptionType Type,
	string Name,
	string ActionText,
	Pixmap Icon,
	Action action )
	: Option( Type, Name, ActionText, Icon )
{
	public static List<ActionOption> All()
	{
		List<ActionOption> options = new();

		foreach ( var entry in CreateAsset.BuiltIn )
		{
			// options.Add( new ActionOption(
			// 	OptionType.Action,
			// 	"Action",
			// 	
			// 	) );
		}

		foreach ( var gameResource in EditorTypeLibrary.GetAttributes<GameResourceAttribute>().OrderBy( x => x.Name ) )
		{
			Action<string> createResource = s =>
			{
				var asset = AssetSystem.CreateResource( gameResource.Extension, s );
				// MainAssetBrowser.Instance?.FocusOnAsset( asset );
				// EditorUtility.InspectorObject = asset;
			};

			var asset = AssetType.FromType( gameResource.TargetType );

			options.Add( new ActionOption(
				OptionType.Action,
				$"New {asset.FriendlyName}..",
				"Action",
				asset.Icon64,
				() =>
				{
					var fd = new FileDialog( null );
					fd.Title = $"Create {gameResource.Name}";
					fd.Directory = Project.Current.Path;
					fd.DefaultSuffix = $".{gameResource.Extension}";
					fd.SelectFile( $"untitled.{gameResource.Extension}" );
					fd.SetFindFile();
					fd.SetModeSave();
					fd.SetNameFilter( $"{gameResource.Name} (*.{gameResource.Extension})" );

					if ( !fd.Execute() )
						return;

					Log.Info( "executed" );

					createResource( fd.SelectedFile );
				}
			) );
		}

		return options;
	}
}
