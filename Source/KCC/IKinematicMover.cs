using FlaxEngine;

namespace KCC;

/// <summary>
/// </summary>
public interface IKinematicMover
{
	/// <summary>
	/// Callback to inform the system of the kinematic mover's wished position and orientation
	/// </summary>
	/// <param name="goalPosition"></param>
	/// <param name="goalOrientation"></param>
	public void KinematicUpdate(out Vector3 goalPosition, out Quaternion goalOrientation);
}