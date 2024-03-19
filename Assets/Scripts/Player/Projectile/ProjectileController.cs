using UnityEngine;
using ServiceLocator.Wave.Bloon;
using ServiceLocator.Main;

namespace ServiceLocator.Player.Projectile
{
    public class ProjectileController
    {
        private PlayerService m_PlayerService;
        private ProjectileView m_ProjectileView;
        private ProjectileScriptableObject m_ProjectileScriptableObject;

        private BloonController m_Target;
        private ProjectileState m_CurrentState;

        public ProjectileController(PlayerService playerService, ProjectileView projectilePrefab, Transform projectileContainer)
        {
            this.m_PlayerService = playerService;
            m_ProjectileView = Object.Instantiate(projectilePrefab, projectileContainer);
            m_ProjectileView.SetController(this);
        }

        public void Init(ProjectileScriptableObject projectileScriptableObject)
        {
            this.m_ProjectileScriptableObject = projectileScriptableObject;
            m_ProjectileView.SetSprite(projectileScriptableObject.Sprite);
            m_ProjectileView.gameObject.SetActive(true);
            m_Target = null;
        }

        public void SetPosition(Vector3 spawnPosition) => m_ProjectileView.transform.position = spawnPosition;

        public void SetTarget(BloonController target)
        {
            this.m_Target = target;
            SetState(ProjectileState.ACTIVE);
            RotateTowardsTarget();
        }

        private void RotateTowardsTarget()
        {
            Vector3 direction = m_Target.Position - m_ProjectileView.transform.position;
            float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + 180;
            m_ProjectileView.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        public void UpdateProjectileMotion()
        {
            if(m_Target != null && m_CurrentState == ProjectileState.ACTIVE)
                m_ProjectileView.transform.Translate(Vector2.left * m_ProjectileScriptableObject.Speed * Time.deltaTime, Space.Self);
        }

        public void OnHitBloon(BloonController bloonHit)
        {
            if (m_CurrentState != ProjectileState.ACTIVE)
                return;

            switch (m_ProjectileScriptableObject.Type)
            {
                case ProjectileType.Dart:
                case ProjectileType.Shuriken:
                case ProjectileType.Bullet:
                    TryPointDamage(bloonHit);
                    ResetProjectile();
                    SetState(ProjectileState.HIT_TARGET);
                    break;

                case ProjectileType.Canon:
                    TryAreaDamage();
                    ResetProjectile();
                    SetState(ProjectileState.HIT_TARGET);
                    break;

                case ProjectileType.EnergyBall:
                    TryPointDamage(bloonHit);
                    if (bloonHit.GetBloonType() == BloonType.Blue ||
                        bloonHit.GetBloonType() == BloonType.Red)
                    {
                        return;
                    }
                    ResetProjectile();
                    SetState(ProjectileState.HIT_TARGET);
                    break;

                default:
                    break;
            }
            
        }

        public void ResetProjectile()
        {
            m_Target = null;
            m_ProjectileView.gameObject.SetActive(false);
            m_PlayerService.ReturnProjectileToPool(this);
        }

        private void TryPointDamage(BloonController bloonHit)
        {
            bloonHit.TakeDamage(m_ProjectileScriptableObject.Damage);
        }

        private void TryAreaDamage()
        {
            if (!TryGetNearbyBloons(out BloonController[] bloons))
                return;

            foreach (BloonController bloon in bloons)
            {
                bloon.TakeDamage(m_ProjectileScriptableObject.Damage);
            }
        }

        private bool TryGetNearbyBloons(out BloonController[] bloons)
        {
            Collider2D[] results = new Collider2D[10];
            int hitCount = Physics2D.OverlapCircleNonAlloc(
                m_ProjectileView.transform.position,
                m_ProjectileScriptableObject.AreaDamageRadius,
                results, 
                1 << m_Target.LayerMask);

            if (hitCount <= 0)
            {
                bloons = null;
                return false;
            }

            bloons = new BloonController[hitCount];
            for (int i = 0; i < hitCount; i++)
            {
                bloons[i] = results[i].gameObject.GetComponent<BloonView>().Controller;
            }
            return true;
        }

        private void SetState(ProjectileState newState) => m_CurrentState = newState;
    }

    public enum ProjectileState
    {
        ACTIVE,
        HIT_TARGET
    }
}