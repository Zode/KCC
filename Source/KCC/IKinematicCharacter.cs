using FlaxEngine;

namespace KCC;
#nullable enable

/// <summary>
/// The required interface for controlling KinematicCharacterController.
/// </summary>
public interface IKinematicCharacter
{
	/// <summary>
	/// Called when the simulation needs to know the velocity and orientation for a tick before sweeping movement,
	/// the character will attempt to move until the length of the velocity is more or less zero.
	/// You may transfer root motion to the system by extracting it from the animation and applying it here.
	/// </summary>
	/// <param name="velocity"></param>
	/// <param name="orientation"></param>
	public void KinematicMoveUpdate(out Vector3 velocity, out Quaternion orientation);
	/// <summary>
	/// Called the the character velocity needs to be projected along side the current ground plane during the sweep,
	/// the velocity supplied here is the remaining velocity for the tick at the point where this callback is triggered.
	/// This is necessary if you wish to move up sloped surfaces without issues.
	/// Vector3’s ProjectOnPlane will suffice for modern use.
	/// Tip: the KinematicCharacterController supplies the function “GroundTangent” to help with retro style projection where the ground normal does not affect any lateral speed.
	/// </summary>
	/// <param name="velocity">Current velocity</param>
	/// <param name="gravityEulerNormalized">Current normalized gravity as euler angles</param>
	/// <returns>Velocity</returns>
	public Vector3 KinematicGroundProjection(Vector3 velocity, Vector3 gravityEulerNormalized);
	/// <summary>
	/// Called when the character collides with something during a sweep, this can be used to precisely filter out collisions (eg. teammates).
	/// </summary>
	/// <param name="other"></param>
	/// <returns>True if should collide, False if should pass through</returns>
	public bool KinematicCollisionValid(Collider other);
	/// <summary>
	/// Called when the character collides with something during a sweep,
	/// this may be useful if you need to have something external react to the collision as the final position of the controller’s collider may not actually end up colliding with whatever it hit at the end of the sweep.
	/// </summary>
	/// <param name="hit"></param>
	public void KinematicCollision(RayCastHit hit);
	/// <summary>
	/// Called when the character unstucks itself during a sweep,
	/// this may be useful if you want to implement crushers for example.
	/// </summary>
	/// <param name="collider"></param>
	/// <param name="penetrationDirection"></param>
	/// <param name="penetrationDistance"></param>
	public void KinematicUnstuckEvent(Collider collider, Vector3 penetrationDirection, float penetrationDistance);
	/// <summary>
	/// Called when the character’s ground state changes during a sweep,
	/// this may be useful if you wish to implement particle effects upon landing on ground for example.
	/// </summary>
	/// <param name="groundingState">The new state</param>
	/// <param name="hit">Hit from grounding check, null if ungrounding</param>
	public void KinematicGroundingEvent(GroundState groundingState, RayCastHit? hit);
	/// <summary>
	/// Called during the sweep to check if the character can attach to a rigidbody to move with it.
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
	/// Called for every tick the character is attached to a rigidbody to move with, 
	/// may be useful if you wish to rotate the camera along side the rigidbody it is attached to.
	/// </summary>
	/// <param name="rigidBody"></param>
	public void KinematicAttachedRigidBodyUpdate(RigidBody rigidBody);
	/// <summary>
	/// Called during the sweep when the character collides with a rigidbody, may be useful if you wish to have rigidbodies react interactively and you want to handle it yourself.
	/// </summary>
	/// <param name="rbInteraction"></param>
	public void KinematicRigidBodyInteraction(RigidBodyInteraction rbInteraction);
	/// <summary>
	/// Called when the controller has done all the movement processing.
	/// </summary>
	public void KinematicPostUpdate();
}