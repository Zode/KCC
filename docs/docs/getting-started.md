# Getting Started

To begin, you must first create a `KCC Settings` file, or use the default file included in the plugin, and add it to the custom settings list with the key `KCC Settings`

![ Example image of the custom settings ](/KCC/images/settings.png)

This is required for the plugin to function, and contains some overall settings you may want to adjust.

After making your kinematic character, you need to make a script that implements the [IkinematicCharacterController](/api/KCC.IKinematicCharacter.html) interface and assign the script to the controller’s `Controller` property (this can be simply done by just assigning it on the script’s OnEnable, check the [KCCExample repository](https://github.com/Zode/KCCExample).

> [!TIP]
> This method of assigning interfaces means you can hotswap what script is controlling the controller, which may be useful in case you need to swap a character between AI control and Player control.

This interface will provide you with the necessary callbacks to alter the behavior during runtime.
**The controller MUST have an uniform scale of 1**, if you wish to to resize the character please update the relevant properties in the `KinematicCharacterController`.

> [!NOTE]
> This plugin overrides the RigidBody/Transform’s Position and Orientation properties, KinematicBase’s TransientPosition and TransientOrientation properties can be used in place of those.
>
>If you wish to forcibly set the position or forcibly set the orientation of the kinematic object (eg. teleporting), you need to use KinematicBase’s functions “SetPosition” and “SetOrientation”. Both KinematicMover and KinematicCharacterController extend from KinematicBase.

While the system does execute during a `FixedUpdate` tick and as such it usually does work with code that also relies on FixedUpdate, it has its own order of internal processing. If you need to somehow exactly step in time with the system outside of the callbacks, you may attach yourself to the exposed Actions in the KCC plugin. See [KCC Update loop](kccloop.md).

> [!TIP]
> The KinematicBase that all KCC Actors inherit from contains some useful information.
> You may use the `InitialPosition` and `InitialOrientation` properties to get the information from before any movement processing has happened, and `TransientPosition` and `TransientOrientation` properties to get the information from after any movement processing has happened.

> [!TIP]
> The `KinematicCharacterController` exposes some helper functions for smoother development, most notably: `OverlapCollider` which can be used to query all overlaps with optional inflation for collider size, `CastCollider` which can be used to do a standard collider cast.
>
> Both of these functions automatically select the correct collider shape that matches the controller’s shape.
>
> `ForceUnground `for jumping and other purposes, `IsNormalStableGround` for querying if a normal is considered stable with the controller’s settings, and `GroundTangent` which works similarly to Vector3’s `ProjectOnPlane` without any of the lateral movement.
>
> Additionally for debugging purposes `DebugDrawCollider` is exposed for quickly drawing a wire shape that matches the controller’s shape. **If you use this function you need to wrap it in `FLAX_EDITOR` define guards.**