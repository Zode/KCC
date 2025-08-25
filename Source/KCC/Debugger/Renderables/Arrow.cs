#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger.Renderables;

/// <inheritdoc />
public class Arrow : Renderable
{
	/// <inheritdoc />
	public override void Render()
	{
		DebugDraw.DrawWireArrow(StartPosition, Orientation, Radius, Height, OutlineColor, Time.DeltaTime, DepthTest);
	}
}

#endif