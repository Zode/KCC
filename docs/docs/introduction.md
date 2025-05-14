# Introduction
This character controller provides a more classic approach to movement similar that of Valve’s Half-Life or Croteams’s Serious Sam, however replicating this in modern engines can be a bit of a pain as the built in controllers usually fall flat with tiny issues that lessen from the user experience, and full rigidbody controllers can be hard to execute properly.

This plugin aims to provide a ready to go package to act as an alternative to the built in character controller found in Flax Engine, based on “[Improved Collision detection and Response](https://www.peroxide.dk/papers/collision/collision.pdf)” by Kasper Fauerby and “[Improving the Numerical Robustness of Sphere Swept Collision Detection](https://arxiv.org/ftp/arxiv/papers/1211/1211.0059.pdf)” by Jeff Linahan. 

To avoid re-inventing the wheel where possible (and to provide similarity to users) this plugin also pulls in inspiration from the free “[Kinematic Character Controller](https://assetstore.unity.com/packages/tools/physics/kinematic-character-controller-99131)” for Unity by Philippe St-Amand by mirroring the idea of using interfaces to control the `KinematicMover`s and `KinematicCharacterController`s, along side pulling other ideas like using a callback to filter collisions.

> [!NOTE]
> You will need to provide your own logic for actually moving the character, this is simply just a “lower level” alternative to the built in character controller