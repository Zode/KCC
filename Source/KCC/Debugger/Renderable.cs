#if FLAX_EDITOR
using FlaxEngine;

namespace KCC.Debugger;

/// <summary>
/// Represents a debug renderable.
/// </summary>
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
	/// Start position of the renderable in world space.
	/// </summary>
	public Vector3 StartPosition {get; set;} = Vector3.Zero;
	/// <summary>
	/// End position of the renderable in world space.
	/// </summary>
	public Vector3 EndPosition {get; set;} = Vector3.Zero;
	/// <summary>
	/// Orientation of the renderable in world space.
	/// </summary>
	public Quaternion Orientation {get; set;} = Quaternion.Identity;
	/// <summary>
	/// The object oriented bounding box used for boxes.
	/// </summary>
	public OrientedBoundingBox OrientedBoundingBox {get; set;} = OrientedBoundingBox.Default;
	/// <summary>
	/// The radius of a capsule or sphere.
	/// The scale of a wirearrow.
	/// </summary>
	public float Radius {get; set;} = 0.0f;
	/// <summary>
	/// The height of a capsule.
	/// The cap scale of a wirearrow.
	/// </summary>
	public float Height {get; set;} = 0.0f;
	/// <summary>
	/// Vertex buffer used for meshes.
	/// </summary>
	public Float3[] Vertices {get; set;} = [];
	/// <summary>
	/// Index buffer used for meshes.
	/// </summary>
	public int[] Indices {get; set;} = [];

	/// <summary>
	/// Render this debug renderable.
	/// </summary>
	public abstract void Render();
}
#endif