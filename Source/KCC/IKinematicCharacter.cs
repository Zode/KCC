using FlaxEngine;

namespace KCC;
#nullable enable

/// <summary>
/// The required interface for controlling KinematicCharacterController.
/// </summary>
public interface IKinematicCharacter
{
	/// <summary>
	/// Callback to inform the simulation of the kinematic character's wished velocity and forward orientation during the move pass for a tick.
	/// </summary>
	/// <param name="velocity"></param>
	/// <param name="orientation"></param>
	public void KinematicMoveUpdate(out Vector3 velocity, out Quaternion orientation);
	/// <summary>
	/// Callback to handle ground movement projection.
	/// </summary>
	/// <param name="velocity">Current velocity</param>
	/// <param name="gravityEulerNormalized">Current normalized gravity as euler angles</param>
	/// <returns>Velocity</returns>
	public Vector3 KinematicGroundProjection(Vector3 velocity, Vector3 gravityEulerNormalized);
	/// <summary>
	/// Callback to inform if a collider is something we should collide with or pass through.
	/// </summary>
	/// <param name="other"></param>
	/// <returns>True if should collide, False if should pass through</returns>
	public bool KinematicCollisionValid(Collider other);
	/// <summary>
	/// Called when the controller hits any surface.
	/// </summary>
	/// <param name="hit"></param>
	public void KinematicCollision(RayCastHit hit);
	/// <summary>
	/// Called when a collider is penetrated with during unstuck solve.
	/// </summary>
	/// <param name="collider"></param>
	/// <param name="penetrationDirection"></param>
	/// <param name="penetrationDistance"></param>
	public void KinematicUnstuckEvent(Collider collider, Vector3 penetrationDirection, float penetrationDistance);
	/// <summary>
	/// Called when the grounding state changes.
	/// </summary>
	/// <param name="groundingState">The new state</param>
	/// <param name="hit">Hit from grounding check, null if ungrounding</param>
	public void KinematicGroundingEvent(GroundState groundingState, RayCastHit? hit);
	/// <summary>
	/// Callback to inform if a rigidbody can be attached to.
	/// </summary>
	/// <param name="rigidBody"></param>
	/// <returns>True if can attach, False if not</returns>
	public bool KinematicCanAttachToRigidBody(RigidBody rigidBody);
	/// <summary>
	/// Called when the character attaches or detaches itself from a rigidbody.
	/// </summary>
	/// <param name="attached">True if attaching, False if detaching</param>
	/// <param name="rigidBody">Rigidbody being attached to, or being detached from</param>
	public void KinematicAttachedRigidBodyEvent(bool attached, RigidBody? rigidBody);
	/// <summary>
	/// Called for every tick the controller is attached to a rigidbody.
	/// Useful for orienting camera on rotating platforms.
	/// </summary>
	/// <param name="rigidBody"></param>
	public void KinematicAttachedRigidBodyUpdate(RigidBody rigidBody);
	/// <summary>
	/// Called when the controller is set to manual rigidbody interaction mode, leaving the handling to the script.
	/// </summary>
	/// <param name="rbInteraction"></param>
	public void KinematicRigidBodyInteraction(RigidBodyInteraction rbInteraction);
	/// <summary>
	/// Called when the controller has done all the movement processing.
	/// </summary>
	public void KinematicPostUpdate();
}