namespace Pirate.Ship;

[GameResource( "Ship Attachment", "shippart", "Definition for a ship attachment", Icon = "extension" )]
public sealed class ShipAttachment : GameResource
{
	public Model? Model { get; set; }
	public string? ModelAttachmentName { get; set; }
	public Vector3 Position { get; set; }
	public Rotation Rotation { get; set; }
	public bool ShowByDefault { get; set; }
}
