using UnityEngine;

[RequireComponent(typeof(HealthModel))]
public class HealthView : MonoBehaviour
{
    private HealthModel _healthModel;
    private HealthSystem _healthSystem;
    private void Awake()
    {
        _healthModel = GetComponent<HealthModel>();
    }
    public void OnHealthUpdated(float health)
    {
        _healthModel.HealthText.text = $"Health: {health}";
    }
}