using FlaxEngine;

namespace KCC;

/// <summary>
/// Container for KCC character's RigidBody interactions.
/// </summary>
public struct RigidBodyInteraction
{
	/// <summary>
	/// The rigidbody hit.
	/// </summary>
	public RigidBody RigidBody;
	/// <summary>
	/// The world position where the collision happened.
	/// </summary>
	public Vector3 Point;
	/// <summary>
	/// The normal of the collision.
	/// </summary>
	public Vector3 Normal;
	/// <summary>
	/// The speed upon which we collided with.
	/// </summary>
	public Vector3 CharacterVelocity;
	/// <summary>
	/// The speed that the rigidbody had when we collided with it.
	/// </summary>
	public Vector3 BodyVelocity;
}