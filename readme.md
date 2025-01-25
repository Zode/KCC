# Kinematic Character Controller 
For Flax Engine as a plugin, ideal for retro feel games!

Pull requests & suggestions are welcome!

This is a KCC implementation based on:
[Improved Collision detection and Response](https://www.peroxide.dk/papers/collision/collision.pdf), [Improving the Numerical Robustness of Sphere Swept Collision Detection](https://arxiv.org/ftp/arxiv/papers/1211/1211.0059.pdf), with the following borrowed:

- Feature ideas, Interface controller idea, kinematic platform mover, rigidbody move-with calculations from [Kinematic Character Controller (for unity)](https://assetstore.unity.com/packages/tools/physics/kinematic-character-controller-99131)
- Dynamic rigidbody pushing calculation from [OpenKCC (for unity)](https://github.com/nicholas-maltbie/OpenKCC)
- Crease fix from [Quake 3](https://github.com/id-Software/Quake-III-Arena/blob/dbe4ddb10315479fc00086f08e25d968b4b43c49/code/game/bg_slidemove.c#L130)

## Features:

- Basic iterative swept kinematic movement
- Multiple collider types (box, capsule, sphere)
- Ability to filter colliders on a per collider level
- Stair stepping!
- Moving with rigidbodies / kinematic movers
- Rigidbody interaction (pushing them around)
- Arbitrary gravity!

An example project can be found at: https://github.com/Zode/KCCExample