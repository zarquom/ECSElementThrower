using Unity.Entities;
using UnityEngine;
using Unity.Physics.Authoring;
using Unity.Mathematics;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlatformMovementAuthoring : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private Vector3 _movementVector;

    public float MovementSpeed => _movementSpeed;
    public Vector3 MovementVector => _movementVector;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Color visualizationColor = new Color(.4f, .4f, .9f);
        Vector3 initialPosition = transform.position;
        Vector3 startPosition = initialPosition - _movementVector;
        Vector3 endPosition = initialPosition + _movementVector;
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, visualizationColor, null, 5f);
        var shape = GetComponent<PhysicsShapeAuthoring>();
        float3 platformSize = shape.GetBoxProperties().Size * transform.localScale;
        Color cachedColor = Handles.color;
        Handles.color = visualizationColor;
        Handles.DrawWireCube(startPosition, platformSize);
        Handles.DrawWireCube(endPosition, platformSize);
        Handles.color = cachedColor;
    }
#endif
}

public class PlatformMovementBaker : Baker<PlatformMovementAuthoring>
{
    public override void Bake(PlatformMovementAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new PlatformMovementComponentData
        {
            InitialPosition = authoring.transform.position,
            MovementSpeed = authoring.MovementSpeed,
            MovementVector = authoring.MovementVector,
            IsReverseMovement = false
        });
    }
}