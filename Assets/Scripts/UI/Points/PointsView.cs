using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent (typeof(PointsModel))]
public class PointsView : MonoBehaviour
{
    private PointsModel _pointsModel;

    public void UpdateView(float previousPoints, float points)
    {
        DOVirtual.Float(previousPoints, points, _pointsModel.PointsAnimationTime, value => { _pointsModel.PointsText.text = $"Points: {value:F}"; });
    }

    private void Awake()
    {
        _pointsModel = GetComponent<PointsModel>();
    }
}
