namespace KCC;

/// <summary>
/// RigidBody moving mode for KCC characters.
/// Determines how the KCC character should move when standing on top of a RigidBody.
/// </summary>
public enum RigidBodyMoveMode
{
	/// <summary>
	/// Disable moving along with RigidBodies.
	/// </summary>
	None,
	/// <summary>
	/// Move only with kinematic mover RigidBodies.
	/// </summary>
	KinematicMoversOnly,
	/// <summary>
	/// Move with all RigidBodies.
	/// </summary>
	All,
}