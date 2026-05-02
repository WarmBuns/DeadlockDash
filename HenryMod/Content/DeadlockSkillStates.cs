using EntityStates;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using R2API;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;
using DeadlockDash.Components;
using DeadlockDashState = DeadlockDash.SkillStates.DeadlockDash;

namespace DeadlockDash.Content
{
    public static class DeadlockSkillStates
    {
        public const string DashSkillSlotName = "DeadlockDashSkill";
        public const string StateMachineName = "DeadlockStates";

        public static SkillDef DashSkillDef { get; private set; }

        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(DeadlockDashState));
            DashSkillDef = CreateDashSkillDef();
            if (!DashSkillDef)
            {
                Log.Error("Failed to create DeadlockDash SkillDef.");
                return;
            }
            
            On.RoR2.BodyCatalog.Init += BodyCatalog_Init;

        }

        private static IEnumerator BodyCatalog_Init(On.RoR2.BodyCatalog.orig_Init orig)
        {
            AddDashToSurvivors();
            return orig();
        }

        private static SkillDef CreateDashSkillDef()
        {
            SkillDef skillDef = Modules.Skills.CreateSkillDef(new Modules.SkillDefInfo
            {
                skillName = "DeadlockDash",
                skillNameToken = DeadlockDashLanguage.SkillNameToken,
                skillDescriptionToken = DeadlockDashLanguage.SkillDescriptionToken,
                activationState = new SerializableEntityStateType(typeof(DeadlockDashState)),
                activationStateMachineName = StateMachineName,
                interruptPriority = InterruptPriority.PrioritySkill,
                baseRechargeInterval = Modules.Config.DashCooldown.Value,
                baseMaxStock = Mathf.Max(1, Modules.Config.DashStocks.Value),
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
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,
            });

            Modules.Config.ApplyToSkill(skillDef);
            if (skillDef.icon == null)
            {
                Log.Debug("DeadlockDash skill is using a null icon sprite.");
            }

            return skillDef;
        }

        public static void AddDashToSurvivors()
        {
            if (!DashSkillDef)
            {
                Log.Error("AddDashToSurvivors was called before DeadlockDash SkillDef was initialized.");
                return;
            }

            SurvivorDef[] survivorDefs = ContentManager.survivorDefs;
            if (survivorDefs == null || survivorDefs.Length == 0)
            {
                Log.Warning("DeadlockDash injection skipped because ContentManager.survivorDefs was empty.");
                return;
            }

            int injectedCount = 0;
            int malformedCount = 0;

            foreach (SurvivorDef survivorDef in survivorDefs)
            {
                if (survivorDef == null)
                {
                    Log.Warning("Survivordef is null...");
                    malformedCount++;
                    continue;
                }

                GameObject bodyPrefab = survivorDef?.bodyPrefab;
                if (!bodyPrefab)
                {
                    Log.Warning($"Skipping DeadlockDash injection for '{survivorDef}' because it has a null bodyPrefab.");
                    malformedCount++;
                    continue;
                }

                Modules.StateMachines.AddEntityStateMachine(bodyPrefab, StateMachineName);

                if (!bodyPrefab.TryGetComponent(out SkillLocator skillLocator))
                {
                    malformedCount++;
                    Log.Warning($"Skipping DeadlockDash injection for '{bodyPrefab.name}' because it is missing SkillLocator.");
                    continue;
                }

                GenericSkill dashSkill = Modules.Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, DashSkillSlotName, "UniversalSkills", true);
                if (!dashSkill)
                {
                    malformedCount++;
                    Log.Warning($"Failed to create DeadlockDashSkill GenericSkill for '{bodyPrefab.name}'.");
                    continue;
                }
                

                if (dashSkill.skillFamily == null || dashSkill.skillFamily.variants == null)
                {
                    malformedCount++;
                    Log.Warning($"DeadlockDashSkill family setup failed for '{bodyPrefab.name}'.");
                    continue;
                }

                if (!SkillFamilyContainsDef(dashSkill.skillFamily, DashSkillDef))
                {
                    Log.Info("Attempting to add skill to family");
                    Modules.Skills.AddSkillToFamily(dashSkill.skillFamily, DashSkillDef);
                }

                DeadlockSkillsInputDriver inputDriver = bodyPrefab.GetComponent<DeadlockSkillsInputDriver>();
                if (!inputDriver)
                {
                    inputDriver = bodyPrefab.AddComponent<DeadlockSkillsInputDriver>();
                }

                inputDriver.dashSkill = dashSkill;

                if (!inputDriver)
                {
                    malformedCount++;
                    Log.Warning($"Failed to add DeadlockDashInputDriver to '{bodyPrefab.name}'.");
                    continue;
                } else
                {
                    Log.Debug($"Injected Deadlock Skills into '{bodyPrefab.name}' successfully.");
                    injectedCount++;
                }
            }

            Log.Info($"DeadlockDash injection complete. Injected={injectedCount}, Malformed={malformedCount}.");
        }

        private static bool SkillFamilyContainsDef(SkillFamily skillFamily, SkillDef skillDef)
        {
            if (!skillFamily || skillFamily.variants == null || !skillDef)
            {
                return false;
            }

            for (int i = 0; i < skillFamily.variants.Length; i++)
            {
                if (skillFamily.variants[i].skillDef == skillDef)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
