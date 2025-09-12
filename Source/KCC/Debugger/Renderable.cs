#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger;

/// <summary>
/// Represents a debug renderable.
/// </summary>
[HideInEditor]
public abstract class Renderable
{
	/// <summary>
	/// Fill color of this renderable.
	/// </summary>
	public Color FillColor {get; set;} = Color.Transparent;
	/// <summary>
	/// Outline color of this renderable.
	/// </summary>
	public Color OutlineColor {get; set;} = Color.White;
	/// <summary>
	/// If set to true depth test will be performed, otherwise depth will be ignored.
	/// </summary>
	public bool DepthTest {get; set;} = true;
	/// <summary>
	/// (Start) position of the renderable in world space.
	/// </summary>
	public Vector3 Position {get; set;} = Vector3.Zero;
	/// <summary>
	/// Orientation of the renderable in world space.
	/// </summary>
	public Quaternion Orientation {get; set;} = Quaternion.Identity;

	/// <summary>
	/// Render this debug renderable.
	/// </summary>
	public abstract void Render();
}
#endif