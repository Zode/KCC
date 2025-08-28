#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger.Renderables;

/// <inheritdoc />
public class Capsule : Renderable
{
	/// <summary>
	/// The radius of the capsule.
	/// </summary>
	public float Radius {get; set;} = 0.0f;

	/// <summary>
	/// The height of the capsule.
	/// </summary>
	public float Height {get; set;} = 0.0f;

	/// <inheritdoc />
	public override void Render()
	{
		if(FillColor.A > 0.0f)
		{
			DebugDraw.DrawCapsule(Position, Orientation, Radius, Height, FillColor, 0.0f, DepthTest);
		}

		DebugDraw.DrawWireCapsule(Position, Orientation, Radius, Height, OutlineColor, 0.0f, DepthTest);
	}
}

#endif