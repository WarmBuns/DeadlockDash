using EntityStates;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;
using DeadlockDash.Components;
using DeadlockDashState = DeadlockDash.SkillStates.DeadlockDash;

namespace DeadlockDash.Content
{
    internal static class DeadlockDashStates
    {
        internal const string DashSkillSlotName = "DeadlockDashSkill";

        internal static SkillDef DashSkillDef { get; private set; }

        internal static void Init()
        {
            Modules.Content.AddEntityState(typeof(DeadlockDashState));
            DashSkillDef = CreateDashSkillDef();
            if (!DashSkillDef)
            {
                Log.Error("Failed to create DeadlockDash SkillDef.");
                return;
            }
            // On.RoR2.SurvivorCatalog.Init += SurvivorCatalog_Init;
            // TODO: Trying BodyCatalog instead
            On.RoR2.BodyCatalog.Init += BodyCatalog_Init;

            // Focusing on getting the mod working for now
            //CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            //On.RoR2.CharacterBody.OnSkillCooldown += CharacterBody_OnSkillCooldown;
        }


        // private static void SurvivorCatalog_Init(On.RoR2.SurvivorCatalog.orig_Init orig)
        // {
        //     orig();
        //     AddDashToSurvivors();
        // }

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
                activationStateMachineName = "Body",
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
                cancelSprintingOnActivation = false,
                forceSprintDuringState = true,
            });

            Modules.Config.ApplyToSkill(skillDef);
            if (skillDef.icon == null)
            {
                Log.Debug("DeadlockDash skill is using a null icon sprite.");
            }

            return skillDef;
        }

        internal static void SyncDashBuffs(CharacterBody body, GenericSkill dashSkill)
        {
            if (!NetworkServer.active || !body || !IsDashSkill(dashSkill))
            {
                if (NetworkServer.active && body && dashSkill == null)
                {
                    Log.Warning($"SyncDashBuffs called for '{body.name}' with a null dash skill.");
                }

                return;
            }

            if (DeadlockDashBuffs.bdDeadlockDashReady == null || DeadlockDashBuffs.bdDeadlockDashCooldown == null)
            {
                Log.Warning($"SyncDashBuffs skipped for '{body.name}' because dash buff definitions were not initialized.");
                return;
            }

            int readyStacks = Mathf.Clamp(dashSkill.stock, 0, dashSkill.maxStock);
            body.SetBuffCount(DeadlockDashBuffs.bdDeadlockDashReady.buffIndex, readyStacks);
            RebuildCooldownBuffs(body, dashSkill);
        }

        private static void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            if (!NetworkServer.active || !body || !body.isPlayerControlled)
            {
                return;
            }

            GenericSkill dashSkill = FindDashSkill(body);
            if (!IsDashSkill(dashSkill))
            {
                Log.Warning($"Player-controlled body '{body.name}' started without a valid DeadlockDashSkill.");
                return;
            }

            body.ClearTimedBuffs(DeadlockDashBuffs.bdDeadlockDashCooldown);
            body.SetBuffCount(DeadlockDashBuffs.bdDeadlockDashReady.buffIndex, Mathf.Max(1, dashSkill.maxStock));
        }

        private static void CharacterBody_OnSkillCooldown(On.RoR2.CharacterBody.orig_OnSkillCooldown orig, CharacterBody self, GenericSkill skill, int restocks)
        {
            orig(self, skill, restocks);

            if (!NetworkServer.active || !IsDashSkill(skill))
            {
                return;
            }

            SyncDashBuffs(self, skill);
        }

        private static GenericSkill FindDashSkill(CharacterBody body)
        {
            SkillLocator skillLocator = body ? body.skillLocator : null;
            if (body && !skillLocator)
            {
                Log.Warning($"CharacterBody '{body.name}' is missing SkillLocator while resolving DeadlockDashSkill.");
                return null;
            }

            return skillLocator ? skillLocator.FindSkill(DashSkillSlotName) : null;
        }

        private static bool IsDashSkill(GenericSkill skill)
        {
            return skill && (skill.skillDef == DashSkillDef || string.Equals(skill.skillName, DashSkillSlotName, StringComparison.Ordinal));
        }

        private static void RebuildCooldownBuffs(CharacterBody body, GenericSkill dashSkill)
        {
            body.ClearTimedBuffs(DeadlockDashBuffs.bdDeadlockDashCooldown);

            int missingStocks = Mathf.Max(0, dashSkill.maxStock - dashSkill.stock);
            if (missingStocks <= 0 || dashSkill.rechargeStock <= 0)
            {
                return;
            }

            float rechargeInterval = dashSkill.CalculateFinalRechargeInterval();
            if (rechargeInterval <= 0f)
            {
                Log.Warning($"DeadlockDashSkill on '{body.name}' has a non-positive recharge interval ({rechargeInterval}).");
                return;
            }

            float firstCooldownDuration = Mathf.Clamp(rechargeInterval - dashSkill.rechargeStopwatch, 0f, rechargeInterval);
            if (firstCooldownDuration <= 0f)
            {
                firstCooldownDuration = rechargeInterval;
            }

            for (int i = 0; i < missingStocks; i++)
            {
                body.AddTimedBuff(DeadlockDashBuffs.bdDeadlockDashCooldown, firstCooldownDuration + rechargeInterval * i);
            }
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
                GameObject bodyPrefab = survivorDef?.bodyPrefab;
                if (!bodyPrefab)
                {
                    Log.Warning($"Skipping DeadlockDash injection for '{survivorDef}' because it has a null bodyPrefab.");
                    malformedCount++;
                    continue;
                }

                if (EntityStateMachine.FindByCustomName(bodyPrefab, "Body") == null)
                {
                    Log.Warning($"Skipping DeadlockDash injection for '{bodyPrefab.name}' because it is missing a 'Body' EntityStateMachine.");
                    malformedCount++;
                    continue;
                }

                if (!bodyPrefab.TryGetComponent(out SkillLocator skillLocator))
                {
                    malformedCount++;
                    Log.Warning($"Skipping DeadlockDash injection for '{bodyPrefab.name}' because it is missing SkillLocator.");
                    continue;
                }

                GenericSkill dashSkill = skillLocator.FindSkill(DashSkillSlotName);
                if (!dashSkill)
                {
                    dashSkill = Modules.Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, DashSkillSlotName, "UniversalSkills", true);
                    if (!dashSkill)
                    {
                        malformedCount++;
                        Log.Warning($"Failed to create DeadlockDashSkill GenericSkill for '{bodyPrefab.name}'.");
                        continue;
                    }
                }

                if (dashSkill.skillFamily == null || dashSkill.skillFamily.variants == null)
                {
                    malformedCount++;
                    Log.Warning($"DeadlockDashSkill family setup failed for '{bodyPrefab.name}'.");
                    continue;
                }

                if (!SkillFamilyContainsDef(dashSkill.skillFamily, DashSkillDef))
                {
                    Modules.Skills.AddSkillToFamily(dashSkill.skillFamily, DashSkillDef);
                }

                DeadlockDashInputDriver inputDriver = bodyPrefab.GetComponent<DeadlockDashInputDriver>();
                if (!inputDriver)
                {
                    inputDriver = bodyPrefab.AddComponent<DeadlockDashInputDriver>();
                }

                inputDriver.dashSkill = dashSkill;

                if (!inputDriver)
                {
                    malformedCount++;
                    Log.Warning($"Failed to add DeadlockDashInputDriver to '{bodyPrefab.name}'.");
                    continue;
                } else
                {
                    Log.Debug($"Injected DeadlockDash into '{bodyPrefab.name}' successfully.");
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
