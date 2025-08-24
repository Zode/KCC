#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger.Renderables;

/// <inheritdoc />
public class Line : Renderable
{
	/// <inheritdoc />
	public override void Render()
	{
		DebugDraw.DrawLine(StartPosition, EndPosition, OutlineColor, Time.DeltaTime, DepthTest);
	}
}

#endif