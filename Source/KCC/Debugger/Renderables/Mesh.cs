#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger.Renderables;

/// <inheritdoc />
public class Mesh : Renderable
{
	/// <inheritdoc />
	public override void Render()
	{
		if(Vertices.Length == 0 || Indices.Length == 0)
		{
			return;
		}

		if(FillColor.A > 0.0f)
		{
			DebugDraw.DrawTriangles(Vertices, Indices, FillColor, Time.DeltaTime, DepthTest);
		}

		if(OutlineColor.A == 0.0f)
		{
			return;
		}

		for(int i = 0; i < Indices.Length; i += 3)
		{
			DebugDraw.DrawLine(Vertices[Indices[i]], Vertices[Indices[i + 1]], OutlineColor, Time.DeltaTime, DepthTest);
			DebugDraw.DrawLine(Vertices[Indices[i + 1]], Vertices[Indices[i + 2]], OutlineColor, Time.DeltaTime, DepthTest);
			DebugDraw.DrawLine(Vertices[Indices[i + 2]], Vertices[Indices[i + 0]], OutlineColor, Time.DeltaTime, DepthTest);
		}
	}
}

#endif