using UnityEngine;
using ServiceLocator.Wave.Bloon;

namespace ServiceLocator.Player.Projectile
{
    public class ProjectileController
    {
        private ProjectileView projectileView;
        private ProjectileScriptableObject projectileScriptableObject;

        private BloonController target;
        private ProjectileState currentState;

        public ProjectileController(ProjectileView projectilePrefab, Transform projectileContainer)
        {
            projectileView = Object.Instantiate(projectilePrefab, projectileContainer);
            projectileView.SetController(this);
        }

        public void Init(ProjectileScriptableObject projectileScriptableObject)
        {
            this.projectileScriptableObject = projectileScriptableObject;
            projectileView.SetSprite(projectileScriptableObject.Sprite);
            projectileView.gameObject.SetActive(true);
            target = null;
        }

        public void SetPosition(Vector3 spawnPosition) => projectileView.transform.position = spawnPosition;

        public void SetTarget(BloonController target)
        {
            this.target = target;
            SetState(ProjectileState.ACTIVE);
            RotateTowardsTarget();
        }

        private void RotateTowardsTarget()
        {
            Vector3 direction = target.Position - projectileView.transform.position;
            float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + 180;
            projectileView.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        public void UpdateProjectileMotion()
        {
            if(target != null && currentState == ProjectileState.ACTIVE)
                projectileView.transform.Translate(Vector2.left * projectileScriptableObject.Speed * Time.deltaTime, Space.Self);
        }

        public void OnHitBloon(BloonController bloonHit)
        {
            if (currentState == ProjectileState.ACTIVE)
            {
                bloonHit.TakeDamage(projectileScriptableObject.Damage);
                ResetProjectile();
                SetState(ProjectileState.HIT_TARGET);
            }
        }

        public void ResetProjectile()
        {
            target = null;
            projectileView.gameObject.SetActive(false);
            GameService.Instance.PlayerService.ReturnProjectileToPool(this);
        }

        private void SetState(ProjectileState newState) => currentState = newState;

        private enum ProjectileState
        {
            ACTIVE,
            HIT_TARGET
        }
    }
}