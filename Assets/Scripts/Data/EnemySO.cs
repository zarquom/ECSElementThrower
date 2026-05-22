using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "ElemenThrower/Enemies")]
    public class EnemySO : ScriptableObject
    {
        public EnemyType Type;
        public float Damage;
    }
}
