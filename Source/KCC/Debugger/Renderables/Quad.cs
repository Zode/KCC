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

	/// <summary>
	/// Vertex buffer for the quad.
	/// </summary>
	public Float3[] Vertices {get; set;} = [];

	/// <inheritdoc />
	public override void Render()
	{
		if(FillColor.A > 0.0f)
		{
			DebugDraw.DrawTriangles(Vertices, quadIndices, FillColor, 0.0f, DepthTest);
		}

		//this is currently broken and behaves the same as DrawTriangles:
		//DebugDraw.DrawWireTriangles(Vertices, quadIndices, OutlineColor, 0.0f, DepthTest);

		DebugDraw.DrawLine(Vertices[0], Vertices[1], OutlineColor, 0.0f, DepthTest);
		DebugDraw.DrawLine(Vertices[1], Vertices[3], OutlineColor, 0.0f, DepthTest);
		DebugDraw.DrawLine(Vertices[2], Vertices[3], OutlineColor, 0.0f, DepthTest);
		DebugDraw.DrawLine(Vertices[0], Vertices[2], OutlineColor, 0.0f, DepthTest);
	}
}

#endif