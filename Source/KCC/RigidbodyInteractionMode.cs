namespace KCC;

/// <summary>
/// RigidBody interaction mode for KCC characters.
/// Determines how RigidBodies should interact with the KCC character.
/// </summary>
public enum RigidBodyInteractionMode
{
	/// <summary>
	/// Ignore all rigidbody interactions.
	/// </summary>
	None,
	/// <summary>
	/// Kinematic interactions with rigidbodies where mass matters.
	/// </summary>
	SimulateKinematic,
	/// <summary>
	/// Kinematic interactions with rigidbodies where mass is ignored.
	/// </summary>
	PureKinematic,
	/// <summary>
	/// Kinematic interactions are queried through the interface.
	/// </summary>
	Manual,
}