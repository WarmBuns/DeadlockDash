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
            EntityStateMachine[] machines = bodyPrefab.GetComponents<EntityStateMachine>();
            for (int i = machines.Length - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(machines[i]);
            }

            NetworkStateMachine networkMachine = bodyPrefab.GetComponent<NetworkStateMachine>();
            if (networkMachine)
            {
                networkMachine.stateMachines = Array.Empty<EntityStateMachine>();
            }

            CharacterDeathBehavior deathBehavior = bodyPrefab.GetComponent<CharacterDeathBehavior>();
            if (deathBehavior)
            {
                deathBehavior.idleStateMachine = Array.Empty<EntityStateMachine>();
            }

            SetStateOnHurt setStateOnHurt = bodyPrefab.GetComponent<SetStateOnHurt>();
            if (setStateOnHurt)
            {
                setStateOnHurt.idleStateMachine = Array.Empty<EntityStateMachine>();
            }

            CharacterBody body = bodyPrefab.GetComponent<CharacterBody>();
            if (body)
            {
                body.vehicleIdleStateMachine = Array.Empty<EntityStateMachine>();
            }
        }

        public static EntityStateMachine AddEntityStateMachine(GameObject prefab, string machineName, Type mainStateType = null, Type initialStateType = null, bool addToHurt = true, bool addToDeath = true)
        {
            EntityStateMachine entityStateMachine = EntityStateMachine.FindByCustomName(prefab, machineName) ?? prefab.AddComponent<EntityStateMachine>();
            entityStateMachine.customName = machineName;
            entityStateMachine.mainStateType = new SerializableEntityStateType(mainStateType ?? typeof(Idle));
            entityStateMachine.initialStateType = new SerializableEntityStateType(initialStateType ?? typeof(Idle));

            NetworkStateMachine networkMachine = prefab.GetComponent<NetworkStateMachine>();
            if (networkMachine)
            {
                networkMachine.stateMachines = networkMachine.stateMachines.Append(entityStateMachine).ToArray();
            }

            CharacterDeathBehavior deathBehavior = prefab.GetComponent<CharacterDeathBehavior>();
            if (deathBehavior && addToDeath)
            {
                deathBehavior.idleStateMachine = deathBehavior.idleStateMachine.Append(entityStateMachine).ToArray();
            }

            SetStateOnHurt setStateOnHurt = prefab.GetComponent<SetStateOnHurt>();
            if (setStateOnHurt && addToHurt)
            {
                setStateOnHurt.idleStateMachine = setStateOnHurt.idleStateMachine.Append(entityStateMachine).ToArray();
            }

            CharacterBody body = prefab.GetComponent<CharacterBody>();
            if (body)
            {
                body.vehicleIdleStateMachine = body.vehicleIdleStateMachine.Append(entityStateMachine).ToArray();
            }

            return entityStateMachine;
        }

        public static EntityStateMachine AddMainEntityStateMachine(GameObject bodyPrefab, string machineName = "Body", Type mainStateType = null, Type initialStateType = null)
        {
            EntityStateMachine entityStateMachine = EntityStateMachine.FindByCustomName(bodyPrefab, machineName) ?? bodyPrefab.AddComponent<EntityStateMachine>();
            entityStateMachine.customName = machineName;
            entityStateMachine.mainStateType = new SerializableEntityStateType(mainStateType ?? typeof(GenericCharacterMain));
            entityStateMachine.initialStateType = new SerializableEntityStateType(initialStateType ?? typeof(SpawnTeleporterState));

            NetworkStateMachine networkMachine = bodyPrefab.GetComponent<NetworkStateMachine>();
            if (networkMachine)
            {
                networkMachine.stateMachines = networkMachine.stateMachines.Append(entityStateMachine).ToArray();
            }

            CharacterDeathBehavior deathBehavior = bodyPrefab.GetComponent<CharacterDeathBehavior>();
            if (deathBehavior)
            {
                deathBehavior.deathStateMachine = entityStateMachine;
            }

            SetStateOnHurt setStateOnHurt = bodyPrefab.GetComponent<SetStateOnHurt>();
            if (setStateOnHurt)
            {
                setStateOnHurt.targetStateMachine = entityStateMachine;
            }

            return entityStateMachine;
        }
    }
}
