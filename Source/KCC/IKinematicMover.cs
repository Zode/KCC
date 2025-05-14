using FlaxEngine;

namespace KCC;

/// <summary>
/// The required interface for controlling KinematicMover.
/// </summary>
public interface IKinematicMover
{
	/// <summary>
	/// Callback to inform the system of the kinematic mover's wished position and orientation.
	/// </summary>
	/// <param name="goalPosition"></param>
	/// <param name="goalOrientation"></param>
	public void KinematicUpdate(out Vector3 goalPosition, out Quaternion goalOrientation);
}