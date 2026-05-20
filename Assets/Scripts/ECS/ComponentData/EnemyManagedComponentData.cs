using Unity.Entities;
using UnityEngine;

public class EnemyManagedComponentData : ICleanupComponentData
{
    public GameObject GameObject;
    public Transform Transform;
    public Animator Animator;
}
