#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger.Renderables;

/// <inheritdoc />
public class Line : Renderable
{
	/// <summary>
	/// End position of the line in world space.
	/// </summary>
	public Vector3 EndPosition {get; set;} = Vector3.Zero;

	/// <inheritdoc />
	public override void Render()
	{
		DebugDraw.DrawLine(Position, EndPosition, OutlineColor, 0.0f, DepthTest);
	}
}

#endif