#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger.Renderables;

/// <inheritdoc />
public class Text : Renderable
{
	/// <summary>
	/// The text to draw.
	/// </summary>
	public string Content {get; set;} = string.Empty;
	/// <summary>
	/// Font size of the text.
	/// </summary>
	public int Size {get; set;} = 12;

	/// <inheritdoc />
	public override void Render()
	{
		DebugDraw.DrawText(Content, Position, OutlineColor, Size, 0.0f, 1.0f);
	}
}

#endif