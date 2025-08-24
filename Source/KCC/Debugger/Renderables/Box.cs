#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger.Renderables;

/// <inheritdoc />
public class Box : Renderable
{
	/// <inheritdoc />
	public override void Render()
	{
		if(FillColor.A > 0.0f)
		{
			DebugDraw.DrawBox(OrientedBoundingBox, FillColor, Time.DeltaTime, DepthTest);
		}

		DebugDraw.DrawWireBox(OrientedBoundingBox, OutlineColor, Time.DeltaTime, DepthTest);
	}
}

#endif