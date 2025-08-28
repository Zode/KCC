#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger.Renderables;

/// <inheritdoc />
public class Sphere : Renderable
{
	/// <summary>
	/// The radius of the sphere.
	/// </summary>
	public float Radius {get; set;} = 0.0f;

	/// <inheritdoc />
	public override void Render()
	{
		if(FillColor.A > 0.0f)
		{
			DebugDraw.DrawSphere(new(Position, Radius), FillColor, 0.0f, DepthTest);
		}

		DebugDraw.DrawWireSphere(new(Position, Radius), OutlineColor, 0.0f, DepthTest);
	}
}

#endif