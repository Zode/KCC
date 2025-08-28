#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger.Renderables;

/// <inheritdoc />
public class Capsule : Renderable
{
	/// <inheritdoc />
	public override void Render()
	{
		if(FillColor.A > 0.0f)
		{
			DebugDraw.DrawCapsule(StartPosition, Orientation, Radius, Height, FillColor, Time.DeltaTime, DepthTest);
		}

		DebugDraw.DrawWireCapsule(StartPosition, Orientation, Radius, Height, OutlineColor, Time.DeltaTime, DepthTest);
	}
}

#endif