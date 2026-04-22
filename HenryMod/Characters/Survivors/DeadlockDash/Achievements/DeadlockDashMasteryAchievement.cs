using RoR2;
using DeadlockDash.Modules.Achievements;

namespace DeadlockDash.Survivors.DeadlockDash.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    public class DeadlockDashMasteryAchievement : BaseMasteryAchievement
    {
        public const string identifier = DeadlockDashSurvivor.DEADLOCKDASH_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = DeadlockDashSurvivor.DEADLOCKDASH_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => DeadlockDashSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}
