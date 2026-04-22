using BepInEx;
using DeadlockDash.Survivors.DeadlockDash;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
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

            //easy to use logger
            Log.Init(Logger);

            // used when you want to properly set up language folders
            Modules.Language.Init();

            // character initialization
            new DeadlockDashSurvivor().Initialize();

            // make a content pack and add it. this has to be last
            new Modules.ContentPacks().Initialize();
        }
    }
}
