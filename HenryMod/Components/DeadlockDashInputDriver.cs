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

        private void Awake()
        {
            body = GetComponent<CharacterBody>();
        }

        private void Update()
        {
            if (!body || !body.hasEffectiveAuthority || dashSkill == null)
            {
                return;
            }

            FindLocalUser();
            if (localUser == null || localUser.isUIFocused)
            {
                return;
            }

            if (Modules.Config.GetKeyPressed(Modules.Config.DashKeybind))
            {
                Modules.Config.ApplyToSkill(dashSkill.skillDef);
                dashSkill.ExecuteIfReady();
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
