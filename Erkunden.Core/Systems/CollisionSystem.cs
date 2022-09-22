using System;
using System.Collections.Generic;
using System.Linq;
using Erkunden.Core.Physics;
using Erkunden.Core.Physics.CollisionObjects;
using Erkunden.Core.Physics.Solvers;
using Erkunden.Core.Utils;
using Erkunden.ECS;

namespace Erkunden.Core.Systems
{
	public class CollisionSystem : ISystem<GameTime>
	{
		public List<ISolver> Solvers = new List<ISolver>();
		public event Action<Collision, GameTime> OnCollision = null!;

		public void Process(Entity[] entities, GameTime gameTime)
		{
			ICollidable[] colliders = entities.SelectMany(x => x.Components.OfType<ICollidable>()).ToArray();
			ICollidable colliderA;
			Collision collision;
			List<Collision> collisions = new List<Collision>();
			List<Collision> triggers = new List<Collision>();
			collisions.Capacity = colliders.Length / 2;
			for (int c = 0; c < colliders.Length; c++)
			{
				colliderA = colliders[c];
				// We feature a rolling loop that insures only unique comparisons as each element
				// compared against the rest is removed from the set of possible comparators
				foreach (var colliderB in colliders.AsSpan(c + 1, colliders.Length - (c + 1)))
				{
					// Do not perform collision detection on static objects that never move
					if (!(colliderA is RigidBody) && !(colliderB is RigidBody)) continue;
					// Perform the collision test. If no collision, continue to the next iteration
					if (!CollisionHelper.TestCollision(colliderA, colliderB, out collision)) continue;
					// If one of the collision bodies is a trigger, add it to the triggers collection
					if (colliderA is TriggerBody || colliderB is TriggerBody)
						triggers.Add(collision);
					// Otherwise add it to the regular collision collection
					else collisions.Add(collision);
				}
			}

			// Solve collisions but not triggers
			SolveCollisions(collisions, in gameTime);
			// Invoke the collision event handlers for collisions and triggers
			InvokeCollisionEvents(collisions, in gameTime);
			InvokeCollisionEvents(triggers, in gameTime);
		}

		private void SolveCollisions(List<Collision> collisions, in GameTime gameTime)
		{
			foreach (var solver in Solvers)
				solver.Solve(collisions, gameTime);
		}
		private void InvokeCollisionEvents(List<Collision> collisions, in GameTime gameTime)
		{
			foreach (var collision in collisions)
			{
				OnCollision(collision, gameTime);
				collision.ObjA.Collided(in collision, in gameTime);
				collision.ObjB.Collided(in collision, in gameTime);
			}
		}
	}
}
