using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "New Collectible", menuName = "ElemenThrower/Colletibles")]
    public class CollectibleSO : ScriptableObject
    {
        public CollectibleType Type;
        public float Points;
    }
}
