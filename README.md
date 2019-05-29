<b>Vulpes Pooling</b>

A lightweight Object Pooling solution for Unity.

<b>Getting Started</b>
- Clone or download the repo to your local machine
- Open up your Unity Project
- Open the Package Manager window by navigating to 'Window/Package Manager'
- Click on the '+' icon button and 'Add package from disk...'
- Navigate to wherever you installed the local repo and select the 'package.json' file.

<b>How do I use it?</b>
- This Pooling solution is designed to operate in a fire and forget fashion.
- To create a pool of objects simply call 'Pool.Add(GameObject, int*)' with the desired Prefab as the main arg and optionally the number of instances to generate in the pool (for example you may want to add projectiles to the pool based on a weapon's fire rate for optimal results).
- To spawn an object simply call 'Pool.Spawn(GameObject, Vector3*, Quaternion*, Transform*)' with the desired Prefab as the main arg, you can opptionally specify a position, rotation, and parent transform.
- To Despawn something simply call 'Pool.Despawn(GameObject)' with the desired instance as the main arg, if you attempt to Despawn something that isn't tracked by the Pool it will simply be Destroyed.
- To Remove a pool of objects simply call 'Pool.Remove(GameObject)' with the desired Prefab as the main arg, this will clear all inactive instances of the target object from the pool, any active instances will be simply be Destroyed when Pool.Despawn is called on them.
- Additionally an interface called 'ISpawnable' can be implemented into any class and the OnSpawn and OnDespawn methods will be called when Spawning and Despawning an instance. This can be used for resetting values, physics, etc. to their default values between uses.

<i>\* - Denotes optional arguments.</i>
