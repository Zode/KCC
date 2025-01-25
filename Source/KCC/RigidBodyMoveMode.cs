namespace KCC;

/// <summary>
/// </summary>
public enum RigidBodyMoveMode
{
	/// <summary>
	/// Disable moving with rigidbodies
	/// </summary>
	None,
	/// <summary>
	/// Move only with kinematic mover rigidbodies
	/// </summary>
	KinematicMoversOnly,
	/// <summary>
	/// Move with all rigidbodies
	/// </summary>
	All,
}