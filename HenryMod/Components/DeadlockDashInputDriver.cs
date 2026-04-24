using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace DeadlockDash.Components
{
    [RequireComponent(typeof(CharacterBody))]
    internal class DeadlockDashInputDriver : MonoBehaviour
    {
        internal GenericSkill dashSkill;

        private CharacterBody body;
        private LocalUser localUser;
        private bool loggedMissingBody;
        private bool loggedMissingSkillDef;

        private void Awake()
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
        }

        private void Update()
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

            if (!body || !body.isActiveAndEnabled || !body.hasEffectiveAuthority || dashSkill == null || !dashSkill.enabled)
            {
                Log.Warning("Warned on 2nd if in Update");
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

        private void FindLocalUser()
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
    }
}
