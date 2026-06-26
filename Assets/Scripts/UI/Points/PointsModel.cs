using TMPro;
using UnityEngine;

public class PointsModel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _pointsText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private float _pointsAnimationTime = 1f;

    public TextMeshProUGUI PointsText => _pointsText;
    public TextMeshProUGUI TimerText => _timerText;
    public float PointsAnimationTime => _pointsAnimationTime;
}