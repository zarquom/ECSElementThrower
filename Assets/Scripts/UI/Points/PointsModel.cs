using TMPro;
using UnityEngine;

public class PointsModel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _pointsText;
    [SerializeField] private float _pointsAnimationTime = 1f;

    public TextMeshProUGUI PointsText => _pointsText;
    public float PointsAnimationTime => _pointsAnimationTime;
}