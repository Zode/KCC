#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger.Renderables;

/// <inheritdoc />
public class Quad : Renderable
{
	/// <inheritdoc />
	public override void Render()
	{
		Float3[] quadVerts = [
			new Float3(-0.5f, 0.5f, 0.0f),
			new Float3(0.5f, 0.5f, 0.0f),
			new Float3(-0.5f, -0.5f, 0.0f),
			new Float3(0.5f, -0.5f, 0.0f),
		];

		int[] quadIndices = [
			0, 1, 2,
			2, 1, 3,
		];

		for(int i = 0; i < quadVerts.Length; i++)
		{
			quadVerts[i] *= Radius;
			quadVerts[i] *= Orientation;
			quadVerts[i] += new Float3((float)StartPosition.X, (float)StartPosition.Y, (float)StartPosition.Z);
		}

		if(FillColor.A > 0.0f)
		{
			DebugDraw.DrawTriangles(quadVerts, quadIndices, FillColor, Time.DeltaTime, DepthTest);
		}

		//this is currently broken and behaves the same as DrawWireTriangles:
		//DebugDraw.DrawWireTriangles(quadVerts, quadIndices, OutlineColor, 0.0f, DepthTest);

		DebugDraw.DrawLine(quadVerts[0], quadVerts[1], OutlineColor, Time.DeltaTime, DepthTest);
		DebugDraw.DrawLine(quadVerts[1], quadVerts[3], OutlineColor, Time.DeltaTime, DepthTest);
		DebugDraw.DrawLine(quadVerts[2], quadVerts[3], OutlineColor, Time.DeltaTime, DepthTest);
		DebugDraw.DrawLine(quadVerts[0], quadVerts[2], OutlineColor, Time.DeltaTime, DepthTest);
	}
}

#endif