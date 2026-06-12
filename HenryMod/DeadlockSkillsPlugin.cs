using BepInEx;
using DeadlockDash.Content;
using R2API.Utils;
using RoR2;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace DeadlockDash
{
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    public class DeadlockSkillsPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.Buns.DeadlockDash";
        public const string MODNAME = "DeadlockDash";
        public const string MODVERSION = "1.0.0";

        public const string DEVELOPER_PREFIX = "BUNS";

        public static DeadlockSkillsPlugin instance;

        public void Awake()
        {
            instance = this;
            Log.Init(Logger);
            Log.Info($"Initializing {MODNAME} v{MODVERSION}.");

            Modules.Config.Init();
            Modules.Language.Init();
            Content.DeadlockSkillsBuffs.Init();
            Content.DeadlockDashLanguage.Init();
            Content.DeadlockSkillStates.Init();
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            new Modules.ContentPacks().Initialize();
        }

        public void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {

            if (sender.HasBuff(DeadlockSkillsBuffs.dashBuff))
            {
                args.armorAdd += 25;
                args.attackSpeedMultAdd += 0.5f;
            }
        }
    }
}
