using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;

namespace DeadlockDash.Modules
{
    internal static class Content
    {
        internal static void AddSkillDef(SkillDef skillDef)
        {
            if (!skillDef)
            {
                Log.Warning("Attempted to register a null SkillDef.");
                return;
            }

            ContentPacks.skillDefs.Add(skillDef);
        }

        internal static void AddSkillFamily(SkillFamily skillFamily)
        {
            if (!skillFamily)
            {
                Log.Warning("Attempted to register a null SkillFamily.");
                return;
            }

            ContentPacks.skillFamilies.Add(skillFamily);
        }

        internal static void AddEntityState(Type entityState)
        {
            if (entityState == null)
            {
                Log.Warning("Attempted to register a null entity state.");
                return;
            }

            ContentPacks.entityStates.Add(entityState);
        }

        internal static void AddBuffDef(BuffDef buffDef)
        {
            if (!buffDef)
            {
                Log.Warning("Attempted to register a null BuffDef.");
                return;
            }

            ContentPacks.buffDefs.Add(buffDef);
        }

        internal static BuffDef CreateAndAddBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            if (string.IsNullOrWhiteSpace(buffName))
            {
                Log.Warning("CreateAndAddBuff called with an empty buff name.");
            }

            if (!buffIcon)
            {
                Log.Warning($"CreateAndAddBuff('{buffName}') received a null icon sprite.");
            }

            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            AddBuffDef(buffDef);

            return buffDef;
        }

        internal static void AddEffectDef(EffectDef effectDef)
        {
            if (effectDef == null)
            {
                Log.Warning("Attempted to register a null EffectDef.");
                return;
            }

            ContentPacks.effectDefs.Add(effectDef);
        }

        internal static EffectDef CreateAndAddEffectDef(GameObject effectPrefab)
        {
            if (!effectPrefab)
            {
                Log.Warning("CreateAndAddEffectDef called with a null effect prefab.");
                return null;
            }

            EffectDef effectDef = new EffectDef(effectPrefab);
            AddEffectDef(effectDef);
            return effectDef;
        }

        internal static void AddNetworkSoundEventDef(NetworkSoundEventDef networkSoundEventDef)
        {
            if (!networkSoundEventDef)
            {
                Log.Warning("Attempted to register a null NetworkSoundEventDef.");
                return;
            }

            ContentPacks.networkSoundEventDefs.Add(networkSoundEventDef);
        }

        internal static NetworkSoundEventDef CreateAndAddNetworkSoundEventDef(string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                Log.Warning("CreateAndAddNetworkSoundEventDef called with an empty event name.");
                return null;
            }

            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            AddNetworkSoundEventDef(networkSoundEventDef);

            return networkSoundEventDef;
        }

    }
}
