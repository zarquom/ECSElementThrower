using Unity.Entities;
using UnityEngine;

public class PlayerManagedComponentData : ICleanupComponentData
{
    public Animator AnimatorData;
    public GameObject GameObjectData;
    public Transform TransformData;
}
