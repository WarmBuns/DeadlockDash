using EntityStates;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using DeadlockDash.Components;
using DeadlockDashState = DeadlockDash.SkillStates.DeadlockDash;

namespace DeadlockDash.Content
{
    internal static class DeadlockDashStates
    {
        internal static SkillDef DashSkillDef { get; private set; }

        internal static void Init()
        {
            Modules.Content.AddEntityState(typeof(DeadlockDashState));
            DashSkillDef = CreateDashSkillDef();
            RoR2Application.onLoad += AddDashToSurvivors;
        }

        private static SkillDef CreateDashSkillDef()
        {
            SkillDef skillDef = Modules.Skills.CreateSkillDef(new Modules.SkillDefInfo
            {
                skillName = "DeadlockDash",
                skillNameToken = DeadlockDashLanguage.SkillNameToken,
                skillDescriptionToken = DeadlockDashLanguage.SkillDescriptionToken,
                activationState = new SerializableEntityStateType(typeof(DeadlockDashState)),
                activationStateMachineName = "Body",
                interruptPriority = InterruptPriority.PrioritySkill,
                baseRechargeInterval = Modules.Config.DashCooldown.Value,
                baseMaxStock = 1,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = false,
                mustKeyPress = false,
                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = true,
            });

            Modules.Config.ApplyToSkill(skillDef);
            return skillDef;
        }

        private static void AddDashToSurvivors()
        {
            foreach (SurvivorDef survivorDef in SurvivorCatalog.allSurvivorDefs)
            {
                GameObject bodyPrefab = survivorDef?.bodyPrefab;
                if (!bodyPrefab)
                {
                    continue;
                }

                if (EntityStateMachine.FindByCustomName(bodyPrefab, "Body") == null)
                {
                    continue;
                }

                SkillLocator skillLocator = bodyPrefab.GetComponent<SkillLocator>();
                if (!skillLocator)
                {
                    continue;
                }

                if (bodyPrefab.GetComponent<DeadlockDashInputDriver>())
                {
                    continue;
                }

                GenericSkill dashSkill = Modules.Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, "DeadlockDashSkill", "UniversalSkills", true);
                Modules.Skills.AddSkillToFamily(dashSkill.skillFamily, DashSkillDef);

                DeadlockDashInputDriver inputDriver = bodyPrefab.AddComponent<DeadlockDashInputDriver>();
                inputDriver.dashSkill = dashSkill;
            }
        }
    }
}
