# Object Pool

A lightweight Object Pooling solution for Unity.

What's this?
- It's a lightweight Object Pooling solution for games and apps being developed using Unity.

What can I do with the code?
- You can pretty much do whatever you want with the code provided here, just don't go trying to sell it, that's kind of a shitty thing to do given that I've released it here for free.
- You're welcome to integrate it into larger projects and/or frameworks if you want, you're not expected to release your entire code base as a result of integrating this into a larger project or collection of code, that would be a bit silly.

How do I use it?
- This Pooling solution is designed to operate in a fire and forget fashion, to add it to your Unity Project simply clone or copy the 'VulpesPool' folder and its contents into your Project's 'Assets' folder.
- To create a pool of objects simply call 'Pool.Add(GameObject, int*)' with the desired Prefab as the main arg and optionally the number of instances to generate in the pool (for example you may want to add projectiles to the pool based on a weapon's fire rate for optimal results).
- To spawn an object simply call 'Pool.Spawn(GameObject, Vector3*, Quaternion*, Transform*)' with the desired Prefab as the main arg, you can opptionally specify a position, rotation, and parent transform.
- To Despawn something simply call 'Pool.Despawn(GameObject)' with the desired instance as the main arg, if you attempt to Despawn something that isn't tracked by the Pool it will simply be Destroyed.
- To Remove a pool of objects simply call 'Pool.Remove(GameObject)' with the desired Prefab as the main arg, this will clear all inactive instances of the target object from the pool, any active instances will be simply be Destroyed when Pool.Despawn is called on them.
- Additionally an interface called 'ISpawnable' can be implemented into any class and the OnSpawn and OnDespawn methods will be called when Spawning and Despawning an instance. This can be used for resetting values, physics, etc. to their default values between uses.

\* - Denotes optional arguments.
