using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace DeadlockDash.Components
{
    [RequireComponent(typeof(CharacterBody))]
    internal class DeadlockDashInputDriver : MonoBehaviour
    {
        [SerializeField]
        public GenericSkill dashSkill;

        private CharacterBody body;
        private LocalUser localUser;
        private bool loggedMissingBody;
        private bool loggedMissingSkillDef;

        public void Awake()
        {
            if (!TryGetComponent(out body))
            {
                Log.Error($"DeadlockDashInputDriver on '{gameObject.name}' is missing CharacterBody.");
                loggedMissingBody = true;
                enabled = false;
            } else
            {
                Log.Info("We're awake!");
            }

            if (!dashSkill)
            {
                dashSkill = ResolveDashSkill();
                if (!dashSkill)
                {
                    Log.Warning($"DeadlockDashInputDriver on '{gameObject.name}' could not resolve '{Content.DeadlockDashStates.DashSkillSlotName}' in Awake.");
                }
            }
        }

        public void Update()
        {
            if (!body)
            {
                if (!loggedMissingBody)
                {
                    Log.Error($"DeadlockDashInputDriver on '{gameObject.name}' lost its CharacterBody reference.");
                    loggedMissingBody = true;
                }

                return;
            }

            if (!body || !body.isActiveAndEnabled)
            {
                Log.Warning("!body || !body.isActiveAndEnabled");
                return;
            }

            if (!body.hasEffectiveAuthority)
            {
                Log.Warning("!body.hasEffectiveAuthority");
                return;
            }

            if ( dashSkill == null )
            {
                Log.Warning("dashSkill == null");
                return;
            }

            if (!dashSkill.enabled)
            {
                Log.Warning("!dashSkill.enabled");
                return;
            }

            //FindLocalUser();
            //if (localUser == null || localUser.isUIFocused)
            //{
            //    return;
            //}



            if (Input.GetKeyDown(KeyCode.V))
            {
                if (dashSkill.skillDef == null)
                {
                    if (!loggedMissingSkillDef)
                    {
                        Log.Warning($"Dash input received on '{body.name}' but DeadlockDashSkill has no SkillDef assigned.");
                        loggedMissingSkillDef = true;
                    }

                    return;
                }

                loggedMissingSkillDef = false;
                //Modules.Config.ApplyToSkill(dashSkill.skillDef);
                if (dashSkill.CanExecute())
                {
                    dashSkill.ExecuteIfReady();
                }
            }
        }

        public void FindLocalUser()
        {
            if (localUser != null && localUser.cachedBody == body)
            {
                return;
            }

            foreach (LocalUser user in LocalUserManager.readOnlyLocalUsersList)
            {
                if (user.cachedBody == body)
                {
                    localUser = user;
                    return;
                }
            }

            localUser = null;
        }

        private GenericSkill ResolveDashSkill()
        {
            if (TryGetComponent(out SkillLocator skillLocator))
            {
                GenericSkill locatedSkill = skillLocator.FindSkill(Content.DeadlockDashStates.DashSkillSlotName);
                if (locatedSkill)
                {
                    return locatedSkill;
                }
            }

            GenericSkill[] skills = GetComponents<GenericSkill>();
            for (int i = 0; i < skills.Length; i++)
            {
                if (skills[i] && string.Equals(skills[i].skillName, Content.DeadlockDashStates.DashSkillSlotName, System.StringComparison.Ordinal))
                {
                    return skills[i];
                }
            }

            return null;
        }
    }
}
