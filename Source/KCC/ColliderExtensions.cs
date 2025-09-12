using FlaxEngine;

namespace KCC;
#nullable enable

/// <summary>
/// KCC Extensions for Colliders
/// </summary>
public static class ColliderExtensions
{
	/// <summary>
	/// Try and get the kinematic character controller that owns this collider
	/// </summary>
	/// <param name="collider"></param>
	/// <param name="kinematicCharacterController"></param>
	/// <returns><c>true</c> if the collider was owned by a kinematic character controller, otherwise <c>false</c>.</returns>
	public static bool TryGetKinematicCharacter(this PhysicsColliderActor collider, out KinematicCharacterController? kinematicCharacterController)
	{
		if(collider.Parent != null && collider.Parent.Parent != null && collider.Parent.Parent is KinematicCharacterController kccrb)
		{
			kinematicCharacterController = kccrb;
			return true;
		}

		if(collider.Parent != null && collider.Parent is KinematicCharacterController kcc)
		{
			kinematicCharacterController = kcc;
			return true;
		}

		kinematicCharacterController = null;
		return false;
	}

	/// <summary>
	/// Try and get the kinematic character controller that owns this collider
	/// </summary>
	/// <param name="collider"></param>
	/// <param name="kinematicCharacterController"></param>
	/// <returns><c>true</c> if the collider was owned by a kinematic character controller, otherwise <c>false</c>.</returns>
	public static bool TryGetKinematicCharacter(this Collider collider, out KinematicCharacterController? kinematicCharacterController)
	{
		if(collider.Parent != null && collider.Parent.Parent != null && collider.Parent.Parent is KinematicCharacterController kccrb)
		{
			kinematicCharacterController = kccrb;
			return true;
		}

		if(collider.Parent != null && collider.Parent is KinematicCharacterController kcc)
		{
			kinematicCharacterController = kcc;
			return true;
		}

		kinematicCharacterController = null;
		return false;
	}
}