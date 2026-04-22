using BepInEx;
using R2API.Utils;
using RoR2;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace DeadlockDash
{
    //[BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    public class DeadlockDashPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.Buns.DeadlockDash";
        public const string MODNAME = "DeadlockDash";
        public const string MODVERSION = "1.0.0";

        public const string DEVELOPER_PREFIX = "BUNS";

        public static DeadlockDashPlugin instance;

        void Awake()
        {
            instance = this;
            Log.Init(Logger);

            Modules.Config.Init();
            Modules.Language.Init();
            Content.DeadlockDashBuffs.Init();
            Content.DeadlockDashLanguage.Init();
            Content.DeadlockDashStates.Init();
            new Modules.ContentPacks().Initialize();
        }
    }
}
