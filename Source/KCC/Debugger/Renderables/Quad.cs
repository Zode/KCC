#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger.Renderables;

/// <inheritdoc />
public class Quad : Renderable
{
	static readonly int[] quadIndices = [
		0, 1, 2,
		2, 1, 3,
	];

	/// <inheritdoc />
	public override void Render()
	{
		if(FillColor.A > 0.0f)
		{
			DebugDraw.DrawTriangles(Vertices, quadIndices, FillColor, Time.DeltaTime, DepthTest);
		}

		//this is currently broken and behaves the same as DrawTriangles:
		//DebugDraw.DrawWireTriangles(Vertices, quadIndices, OutlineColor, 0.0f, DepthTest);

		DebugDraw.DrawLine(Vertices[0], Vertices[1], OutlineColor, Time.DeltaTime, DepthTest);
		DebugDraw.DrawLine(Vertices[1], Vertices[3], OutlineColor, Time.DeltaTime, DepthTest);
		DebugDraw.DrawLine(Vertices[2], Vertices[3], OutlineColor, Time.DeltaTime, DepthTest);
		DebugDraw.DrawLine(Vertices[0], Vertices[2], OutlineColor, Time.DeltaTime, DepthTest);
	}
}

#endif