using EntityStates;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using DeadlockDash;
using UnityEngine;

namespace DeadlockDash.Modules
{
    internal static class Skills
    {
        public static GenericSkill CreateGenericSkillWithSkillFamily(GameObject targetPrefab, string familyName, bool hidden = false) => CreateGenericSkillWithSkillFamily(targetPrefab, familyName, familyName, hidden);

        public static GenericSkill CreateGenericSkillWithSkillFamily(GameObject targetPrefab, string genericSkillName, string familyName, bool hidden = false)
        {
            if (!targetPrefab)
            {
                Log.Warning($"CreateGenericSkillWithSkillFamily called with a null target prefab for skill '{genericSkillName}'.");
                return null;
            }

            GenericSkill skill = targetPrefab.AddComponent<GenericSkill>();
            skill.skillName = genericSkillName;
            skill.hideInCharacterSelect = hidden;
            skill.hideInLoadoutSelect = hidden;

            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily as ScriptableObject).name = targetPrefab.name + familyName + "Family";
            newFamily.variants = new SkillFamily.Variant[0];

            skill._skillFamily = newFamily;

            Content.AddSkillFamily(newFamily);
            return skill;
        }

        public static void AddSkillToFamily(SkillFamily skillFamily, SkillDef skillDef, UnlockableDef unlockableDef = null)
        {
            if (!skillFamily)
            {
                Log.Warning($"AddSkillToFamily called with a null SkillFamily for skill '{skillDef?.skillName ?? "<null>"}'.");
                return;
            }

            if (!skillDef)
            {
                Log.Warning("AddSkillToFamily called with a null SkillDef.");
                return;
            }

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);

            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                unlockableDef = unlockableDef,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };
        }

        public static void AddSkillsToFamily(SkillFamily skillFamily, params SkillDef[] skillDefs)
        {
            foreach (SkillDef skillDef in skillDefs)
            {
                AddSkillToFamily(skillFamily, skillDef);
            }
        }

        public static SkillDef CreateSkillDef(SkillDefInfo skillDefInfo)
        {
            return CreateSkillDef<SkillDef>(skillDefInfo);
        }

        public static T CreateSkillDef<T>(SkillDefInfo skillDefInfo) where T : SkillDef
        {
            if (skillDefInfo == null)
            {
                Log.Warning($"CreateSkillDef<{typeof(T).Name}> called with null SkillDefInfo.");
                return null;
            }

            //pass in a type for a custom skilldef, e.g. HuntressTrackingSkillDef
            T skillDef = ScriptableObject.CreateInstance<T>();

            skillDef.skillName = skillDefInfo.skillName;
            (skillDef as ScriptableObject).name = skillDefInfo.skillName;
            skillDef.skillNameToken = skillDefInfo.skillNameToken;
            skillDef.skillDescriptionToken = skillDefInfo.skillDescriptionToken;
            skillDef.icon = skillDefInfo.skillIcon;

            skillDef.activationState = skillDefInfo.activationState;
            skillDef.activationStateMachineName = skillDefInfo.activationStateMachineName;
            skillDef.interruptPriority = skillDefInfo.interruptPriority;

            skillDef.baseMaxStock = skillDefInfo.baseMaxStock;
            skillDef.baseRechargeInterval = skillDefInfo.baseRechargeInterval;

            skillDef.rechargeStock = skillDefInfo.rechargeStock;
            skillDef.requiredStock = skillDefInfo.requiredStock;
            skillDef.stockToConsume = skillDefInfo.stockToConsume;

            skillDef.dontAllowPastMaxStocks = skillDefInfo.dontAllowPastMaxStocks;
            skillDef.beginSkillCooldownOnSkillEnd = skillDefInfo.beginSkillCooldownOnSkillEnd;
            skillDef.canceledFromSprinting = skillDefInfo.canceledFromSprinting;
            skillDef.forceSprintDuringState = skillDefInfo.forceSprintDuringState;
            skillDef.fullRestockOnAssign = skillDefInfo.fullRestockOnAssign;
            skillDef.resetCooldownTimerOnUse = skillDefInfo.resetCooldownTimerOnUse;
            skillDef.isCombatSkill = skillDefInfo.isCombatSkill;
            skillDef.mustKeyPress = skillDefInfo.mustKeyPress;
            skillDef.cancelSprintingOnActivation = skillDefInfo.cancelSprintingOnActivation;

            skillDef.keywordTokens = skillDefInfo.keywordTokens;

            DeadlockDash.Modules.Content.AddSkillDef(skillDef);


            return skillDef;
        }
    }

    /// <summary>
    /// class for easily creating skilldefs with default values, and with a field for UnlockableDef
    /// </summary>
    internal class SkillDefInfo
    {
        public string skillName;
        public string skillNameToken;
        public string skillDescriptionToken;
        public string[] keywordTokens = Array.Empty<string>();
        public Sprite skillIcon;

        public SerializableEntityStateType activationState;
        public string activationStateMachineName;
        public InterruptPriority interruptPriority;

        public float baseRechargeInterval;
        public int baseMaxStock = 1;

        public int rechargeStock = 1;
        public int requiredStock = 1;
        public int stockToConsume = 1;

        public bool resetCooldownTimerOnUse = false;
        public bool fullRestockOnAssign = true;
        public bool dontAllowPastMaxStocks = false;
        public bool beginSkillCooldownOnSkillEnd = false;
        public bool mustKeyPress = false;

        public bool isCombatSkill = true;
        public bool canceledFromSprinting = false;
        public bool cancelSprintingOnActivation = true;
        public bool forceSprintDuringState = false;

        #region constructors
        public SkillDefInfo() { }
        /// <summary>
        /// Creates a skilldef for a typical primary.
        /// <para>combat skill, cooldown: 0, required stock: 0, InterruptPriority: Any</para>
        /// </summary>
        public SkillDefInfo(string skillName,
                            string skillNameToken,
                            string skillDescriptionToken,
                            Sprite skillIcon,

                            SerializableEntityStateType activationState,
                            string activationStateMachineName = "Weapon",
                            bool agile = false)
        {
            this.skillName = skillName;
            this.skillNameToken = skillNameToken;
            this.skillDescriptionToken = skillDescriptionToken;
            this.skillIcon = skillIcon;

            this.activationState = activationState;
            this.activationStateMachineName = activationStateMachineName;

            this.cancelSprintingOnActivation = !agile;

            if (agile) this.keywordTokens = new string[] { "KEYWORD_AGILE" };

            this.interruptPriority = InterruptPriority.Any;
            this.isCombatSkill = true;
            this.baseRechargeInterval = 0;

            this.requiredStock = 0;
            this.stockToConsume = 0;

        }
        #endregion construction complete
    }
}
