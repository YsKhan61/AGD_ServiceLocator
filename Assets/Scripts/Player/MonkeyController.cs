using System.Collections.Generic;
using UnityEngine;
using ServiceLocator.Wave.Bloon;
using ServiceLocator.Player.Projectile;
using ServiceLocator.Main;
using ServiceLocator.Sound;

namespace ServiceLocator.Player
{
    public class MonkeyController
    {
        private MonkeyView monkeyView;
        private MonkeyScriptableObject monkeyScriptableObject;
        private ProjectilePool projectilePool;

        private List<BloonController> bloonsInRange;
        private float attackTimer;

        private SoundService soundService;

        public MonkeyController(SoundService soundService, MonkeyScriptableObject monkeyScriptableObject, ProjectilePool projectilePool)
        {
            this.soundService = soundService;

            monkeyView = Object.Instantiate(monkeyScriptableObject.Prefab);
            monkeyView.SetController(this);
            monkeyView.SetTriggerRadius(monkeyScriptableObject.Range);
            
            this.monkeyScriptableObject = monkeyScriptableObject;
            this.projectilePool = projectilePool;
            bloonsInRange = new List<BloonController>();
            ResetAttackTimer();
        }

        public void SetPosition(Vector3 positionToSet) => monkeyView.transform.position = positionToSet;

        public void BloonEnteredRange(BloonController bloon)
        {
            if (CanAttackBloon(bloon.GetBloonType()))
                bloonsInRange.Add(bloon);
        }

        public void BloonExitedRange(BloonController bloon)
        {
            if (CanAttackBloon(bloon.GetBloonType()))
                bloonsInRange.Remove(bloon);
        }

        public bool CanAttackBloon(BloonType bloonType) => monkeyScriptableObject.AttackableBloons.Contains(bloonType);

        public void UpdateMonkey()
        {
            if(bloonsInRange.Count > 0)
            {
                RotateTowardsTarget(bloonsInRange[0]);
                ShootAtTarget(bloonsInRange[0]);
            }
        }

        private void RotateTowardsTarget(BloonController targetBloon)
        {
            Vector3 direction = targetBloon.Position - monkeyView.transform.position;
            float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + 180;
            monkeyView.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void ShootAtTarget(BloonController targetBloon)
        {
            attackTimer -= Time.deltaTime;
            if(attackTimer <= 0)
            {
                ProjectileController projectile = projectilePool.GetProjectile(monkeyScriptableObject.projectileType);
                projectile.SetPosition(monkeyView.transform.position);
                projectile.SetTarget(targetBloon);
                soundService.PlaySoundEffects(Sound.SoundType.MonkeyShoot);
                ResetAttackTimer();
            }
        }

        private void ResetAttackTimer() => attackTimer = monkeyScriptableObject.AttackRate;
    }
}