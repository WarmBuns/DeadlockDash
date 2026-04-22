namespace DeadlockDash.Content
{
    internal static class DeadlockDashLanguage
    {
        internal const string TokenPrefix = DeadlockDashPlugin.DEVELOPER_PREFIX + "_DEADLOCKDASH_";

        internal const string SkillNameToken = TokenPrefix + "SKILL_NAME";
        internal const string SkillDescriptionToken = TokenPrefix + "SKILL_DESCRIPTION";
        internal const string SkillFamilyName = TokenPrefix + "SKILL_FAMILY_NAME";

        internal static void Init()
        {
            Modules.Language.Add(SkillNameToken, "Deadlock Dash");
            Modules.Language.Add(SkillDescriptionToken, "Dash a short distance with a burst of invulnerability.");
            Modules.Language.Add(SkillFamilyName, "Universal Skills");

            // Uncomment when generating language files for translators.
            // Modules.Language.PrintOutput("DeadlockDash.json");
        }
    }
}
