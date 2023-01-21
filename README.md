# Vulpes Pooling

A lightweight Object Pooling solution for Unity.

As of version 1.0.0 of this package, you are now able to install it, along with other Vulpes Software packages via the Unity Package Manager. 

In Unity 2019 LTS and Unity 2020 onwards you can install the package through 'Project Settings/Package Manager'. Under 'Scoped Registries' click the little '+' button and input the following into the fields on the right.

*Name:* Vulpes Software
*URL:* https://registry.npmjs.org
*Scope(s):* com.vulpes

Click 'Apply', now you should be able to access the Vulpes Software registry under the 'My Registries' section in the Package Manager window using the second dropdown in the top left.

## How do I use this package?
- This Pooling solution is designed to operate in a fire and forget fashion.
- To create a pool of objects simply call 'Pool.Add(GameObject, int*)' with the desired Prefab as the main arg and optionally the number of instances to generate in the pool (for example you may want to add projectiles to the pool based on a weapon's fire rate for optimal results).
- To spawn an object simply call 'Pool.Spawn(GameObject, Vector3*, Quaternion*, Transform*)' with the desired Prefab as the main arg, you can opptionally specify a position, rotation, and parent transform.
- To Despawn something simply call 'Pool.Despawn(GameObject)' with the desired instance as the main arg, if you attempt to Despawn something that isn't tracked by the Pool it will simply be Destroyed.
- To Remove a pool of objects simply call 'Pool.Remove(GameObject)' with the desired Prefab as the main arg, this will clear all inactive instances of the target object from the pool, any active instances will be simply be Destroyed when Pool.Despawn is called on them.
- Additionally an interface called 'ISpawnable' can be implemented into any class and the OnSpawn and OnDespawn methods will be called when Spawning and Despawning an instance. This can be used for resetting values, physics, etc. to their default values between uses.

<i>\* - Denotes optional arguments.</i>
