#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger.Renderables;

/// <inheritdoc />
public class Arrow : Renderable
{
	/// <summary>
	/// The cap scale of the wirearrow.
	/// </summary>
	public float CapScale {get; set;} = 0.0f;
	/// <summary>
	/// The scale of the wirearrow.
	/// </summary>
	public float Scale {get; set;} = 0.0f;

	/// <inheritdoc />
	public override void Render()
	{
		DebugDraw.DrawWireArrow(Position, Orientation, Scale, CapScale, OutlineColor, 0.0f, DepthTest);
	}
}

#endif