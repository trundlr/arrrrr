using System;

[Title( "Waves" ), Category( "Pirates" )]
public sealed class Waves : Component
{
	private float _offset;

	public Waves()
	{
		Instance = this;
	}

	public static Waves Instance { get; set; }
	[Property] public float Amplitude { get; set; } = 1f;
	[Property] public float Length { get; set; } = 2f;
	[Property] public float Speed { get; set; } = 1f;

	protected override void OnUpdate()
	{
		_offset += Speed * Time.Delta;
	}

	public float GetWaveHeight( float x )
	{
		return Amplitude * MathF.Sin( x / Length + _offset );
	}
}
