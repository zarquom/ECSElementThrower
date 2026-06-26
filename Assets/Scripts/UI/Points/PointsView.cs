using DG.Tweening;
using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent (typeof(PointsModel))]
public class PointsView : MonoBehaviour
{
    private PointsModel _pointsModel;

    public void UpdateViewPoints(float previousPoints, float points)
    {
        DOVirtual.Float(previousPoints, points, _pointsModel.PointsAnimationTime, value => { _pointsModel.PointsText.text = $"Points: {value:F}"; });
    }
    public void UpdateViewTimer(float timer)
    {
        string formetedRemainingTime = FormatTime(timer);
        _pointsModel.TimerText.text = $"Timer: {formetedRemainingTime}";
    }

    private string FormatTime(float timer)
    {
        TimeSpan span = TimeSpan.FromSeconds(timer);
        return $"{span.Minutes}m {span.Seconds}s";
    }

    private void Awake()
    {
        _pointsModel = GetComponent<PointsModel>();
    }
}
