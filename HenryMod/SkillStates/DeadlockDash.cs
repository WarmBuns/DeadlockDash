using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace DeadlockDash.SkillStates
{
    public class DeadlockDash : BaseSkillState
    {
        public static float duration = 0.5f;
        public static float initialSpeedCoefficient = 5f;
        public static float finalSpeedCoefficient = 2.5f;

        public static string dodgeSoundString = "HenryRoll";
        public static float dodgeFOV = EntityStates.Commando.DodgeState.dodgeFOV;

        private float rollSpeed;
        private Vector3 forwardDirection;
        private Vector3 previousPosition;

        public override void OnEnter()
        {
            base.OnEnter();

            if (!characterBody)
            {
                Log.Warning("DeadlockDash entered without a CharacterBody.");
            }

            if (isAuthority && inputBank && characterDirection)
            {
                forwardDirection = (inputBank.moveVector == Vector3.zero ? characterDirection.forward : inputBank.moveVector).normalized;
            }
            else if (isAuthority)
            {
                Log.Warning($"DeadlockDash on '{gameObject.name}' is missing inputBank or characterDirection while authoritative.");
            }

            RecalculateRollSpeed();

            if (characterMotor && characterDirection)
            {
                characterMotor.velocity.y = 0f;
                characterMotor.velocity = forwardDirection * rollSpeed;
            }

            Vector3 bodyVelocity = characterMotor ? characterMotor.velocity : Vector3.zero;
            previousPosition = transform.position - bodyVelocity;

            //PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", duration);
            Util.PlaySound(dodgeSoundString, gameObject);

            if (NetworkServer.active && characterBody)
            {
                if (characterBody.GetBuffCount(Content.DeadlockDashBuffs.bdDeadlockDashReady) > 0)
                {
                    characterBody.RemoveBuff(Content.DeadlockDashBuffs.bdDeadlockDashReady);
                }

                characterBody.AddTimedBuff(Content.DeadlockDashBuffs.armorBuff, 3f * duration);
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.5f * duration);
            }
        }

        private void RecalculateRollSpeed()
        {
            rollSpeed = moveSpeedStat * Mathf.Lerp(initialSpeedCoefficient, finalSpeedCoefficient, fixedAge / duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            RecalculateRollSpeed();

            if (characterDirection)
            {
                characterDirection.forward = forwardDirection;
            }

            if (cameraTargetParams)
            {
                cameraTargetParams.fovOverride = Mathf.Lerp(dodgeFOV, 60f, fixedAge / duration);
            }

            Vector3 normalized = (transform.position - previousPosition).normalized;
            if (characterMotor && characterDirection && normalized != Vector3.zero)
            {
                Vector3 velocity = normalized * rollSpeed;
                float forwardScale = Mathf.Max(Vector3.Dot(velocity, forwardDirection), 0f);
                velocity = forwardDirection * forwardScale;
                velocity.y = 0f;
                characterMotor.velocity = velocity;
            }

            previousPosition = transform.position;

            if (isAuthority && fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (cameraTargetParams)
            {
                cameraTargetParams.fovOverride = -1f;
            }

            if (NetworkServer.active && activatorSkillSlot == null && characterBody)
            {
                Log.Warning($"DeadlockDash OnExit on '{characterBody.name}' has no activatorSkillSlot for buff sync.");
            }

            Content.DeadlockDashStates.SyncDashBuffs(characterBody, activatorSkillSlot);
            base.OnExit();

            if (characterMotor)
            {
                characterMotor.disableAirControlUntilCollision = false;
            }
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(forwardDirection);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            forwardDirection = reader.ReadVector3();
        }
    }
}
