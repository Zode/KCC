namespace KCC;

/// <summary>
/// Unstuck behavioral mode for KCC characters.
/// Determines how to solve situations where the physics solve can not solve penetration for non-convex mesh colliders.
/// </summary>
public enum TriangleMeshUnstuckMode
{
	/// <summary>
	/// Do nothing.
	/// </summary>
	None,
	/// <summary>
	/// Push out of triangle mesh object based on its bounding box.
	/// Results in a guaranteed solve, at the cost of allocating a BoxCollider.
	/// </summary>
	BoundingBox,
	/// <summary>
	/// Push out of triangle mesh object by figuring out the closest point on the surface in the direction of the two origin differences.
	/// Does not guarantee a working solve, but gives a nicer result than the bounding box mode.
	/// </summary>
	ClosestPoint,
}