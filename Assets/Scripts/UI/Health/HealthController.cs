using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(HealthModel), typeof(HealthView))]
public class HealthController : MonoBehaviour
{
    private HealthView _healthView;
    private HealthModel _healthModel;
    private HealthSystem _healthSystem;
    private void Awake()
    {
        _healthView = GetComponent<HealthView>();
        _healthModel = GetComponent<HealthModel>();
        _healthSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<HealthSystem>();
    }
    private void OnEnable()
    {
        if (_healthSystem != null)
        {
            _healthSystem.HealthUpdated += OnHealthUpdated;
        }
    }
    private void OnDisable()
    {
        if (_healthSystem != null)
        {
            _healthSystem.HealthUpdated -= OnHealthUpdated;
        }
    }
    private void OnHealthUpdated(float health)
    {
        _healthView.OnHealthUpdated(health);
    }
}