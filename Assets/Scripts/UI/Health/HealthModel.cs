using TMPro;
using UnityEngine;

public class HealthModel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healthText;

    public TextMeshProUGUI HealthText => _healthText;
} 