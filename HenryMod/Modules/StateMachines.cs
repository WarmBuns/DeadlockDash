using EntityStates;
using RoR2;
using System;
using System.Linq;
using UnityEngine;

namespace DeadlockDash.Modules
{
    internal static class StateMachines
    {
        public static void ClearEntityStateMachines(GameObject bodyPrefab)
        {
            if (!bodyPrefab)
            {
                Log.Warning("ClearEntityStateMachines called with a null body prefab.");
                return;
            }

            EntityStateMachine[] machines = bodyPrefab.GetComponents<EntityStateMachine>();
            for (int i = machines.Length - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(machines[i]);
            }

            if (bodyPrefab.TryGetComponent(out NetworkStateMachine networkMachine))
            {
                networkMachine.stateMachines = Array.Empty<EntityStateMachine>();
            }

            if (bodyPrefab.TryGetComponent(out CharacterDeathBehavior deathBehavior))
            {
                deathBehavior.idleStateMachine = Array.Empty<EntityStateMachine>();
            }

            if (bodyPrefab.TryGetComponent(out SetStateOnHurt setStateOnHurt))
            {
                setStateOnHurt.idleStateMachine = Array.Empty<EntityStateMachine>();
            }

            if (bodyPrefab.TryGetComponent(out CharacterBody body))
            {
                body.vehicleIdleStateMachine = Array.Empty<EntityStateMachine>();
            }
        }

        public static EntityStateMachine AddEntityStateMachine(GameObject prefab, string machineName, Type mainStateType = null, Type initialStateType = null, bool addToHurt = true, bool addToDeath = true)
        {
            if (!prefab)
            {
                Log.Warning($"AddEntityStateMachine called with a null prefab for machine '{machineName}'.");
                return null;
            }

            EntityStateMachine entityStateMachine = EntityStateMachine.FindByCustomName(prefab, machineName) ?? prefab.AddComponent<EntityStateMachine>();
            entityStateMachine.customName = machineName;
            entityStateMachine.mainStateType = new SerializableEntityStateType(mainStateType ?? typeof(Idle));
            entityStateMachine.initialStateType = new SerializableEntityStateType(initialStateType ?? typeof(Idle));

            if (prefab.TryGetComponent(out NetworkStateMachine networkMachine))
            {
                networkMachine.stateMachines = networkMachine.stateMachines.Append(entityStateMachine).ToArray();
            }

            if (prefab.TryGetComponent(out CharacterDeathBehavior deathBehavior) && addToDeath)
            {
                deathBehavior.idleStateMachine = deathBehavior.idleStateMachine.Append(entityStateMachine).ToArray();
            }

            if (prefab.TryGetComponent(out SetStateOnHurt setStateOnHurt) && addToHurt)
            {
                setStateOnHurt.idleStateMachine = setStateOnHurt.idleStateMachine.Append(entityStateMachine).ToArray();
            }

            if (prefab.TryGetComponent(out CharacterBody body))
            {
                body.vehicleIdleStateMachine = body.vehicleIdleStateMachine.Append(entityStateMachine).ToArray();
            }

            return entityStateMachine;
        }

        public static EntityStateMachine AddMainEntityStateMachine(GameObject bodyPrefab, string machineName = "Body", Type mainStateType = null, Type initialStateType = null)
        {
            if (!bodyPrefab)
            {
                Log.Warning($"AddMainEntityStateMachine called with a null body prefab for machine '{machineName}'.");
                return null;
            }

            EntityStateMachine entityStateMachine = EntityStateMachine.FindByCustomName(bodyPrefab, machineName) ?? bodyPrefab.AddComponent<EntityStateMachine>();
            entityStateMachine.customName = machineName;
            entityStateMachine.mainStateType = new SerializableEntityStateType(mainStateType ?? typeof(GenericCharacterMain));
            entityStateMachine.initialStateType = new SerializableEntityStateType(initialStateType ?? typeof(SpawnTeleporterState));

            if (bodyPrefab.TryGetComponent(out NetworkStateMachine networkMachine))
            {
                networkMachine.stateMachines = networkMachine.stateMachines.Append(entityStateMachine).ToArray();
            }

            if (bodyPrefab.TryGetComponent(out CharacterDeathBehavior deathBehavior))
            {
                deathBehavior.deathStateMachine = entityStateMachine;
            }

            if (bodyPrefab.TryGetComponent(out SetStateOnHurt setStateOnHurt))
            {
                setStateOnHurt.targetStateMachine = entityStateMachine;
            }

            return entityStateMachine;
        }
    }
}
