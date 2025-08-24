#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger.Renderables;

/// <inheritdoc />
public class Sphere : Renderable
{
	/// <inheritdoc />
	public override void Render()
	{
		if(FillColor.A > 0.0f)
		{
			DebugDraw.DrawWireSphere(new(StartPosition, Radius), FillColor, Time.DeltaTime, DepthTest);
		}

		DebugDraw.DrawWireSphere(new(StartPosition, Radius), OutlineColor, Time.DeltaTime, DepthTest);
	}
}

#endif