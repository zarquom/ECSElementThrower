using System;
using Unity.Entities;
using UnityEngine;

[RequireComponent (typeof(PointsModel), typeof(PointsView))]
public class PointsController : MonoBehaviour
{
    private PointsSystem _pointsSystem;
    private PointsModel _pointsModel;
    private PointsView _pointView;

    private float _previousPoints;

    private void Awake()
    {
        _pointsModel = GetComponent<PointsModel>();
        _pointView = GetComponent<PointsView>();
        _pointsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PointsSystem>();
    }

    private void OnEnable()
    {
        _pointsSystem.PointsUpdated += OnPointsUpdated;
    }

    private void OnPointsUpdated(float points)
    {
        _pointView.UpdateView(_previousPoints, points);
        _previousPoints = points;
    }

    private void OnDisable()
    {
        _pointsSystem.PointsUpdated -= OnPointsUpdated;
    }
}
