using UnityEngine;

namespace ServiceLocator.Player.Projectile
{
    [CreateAssetMenu(fileName = "ProjectileScriptableObject ", menuName = "ScriptableObjects/ProjectileScriptableObject")]
    public class ProjectileScriptableObject : ScriptableObject
    {
        public ProjectileType Type;
        public Sprite Sprite;
        public float Speed;
        public int Damage;
        public float AreaDamageRadius;
    }
}