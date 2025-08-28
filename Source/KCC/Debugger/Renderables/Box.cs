#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger.Renderables;

/// <inheritdoc />
public class Box : Renderable
{
	/// <summary>
	/// The object oriented bounding box.
	/// </summary>
	public OrientedBoundingBox OrientedBoundingBox {get; set;} = OrientedBoundingBox.Default;

	/// <inheritdoc />
	public override void Render()
	{
		if(FillColor.A > 0.0f)
		{
			DebugDraw.DrawBox(OrientedBoundingBox, FillColor, 0.0f, DepthTest);
		}

		DebugDraw.DrawWireBox(OrientedBoundingBox, OutlineColor, 0.0f, DepthTest);
	}
}

#endif