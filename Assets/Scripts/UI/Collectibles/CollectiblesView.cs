using DG.Tweening;
using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[RequireComponent (typeof(CollectiblesModel))]
public class CollectiblesView : MonoBehaviour
{
    private CollectiblesModel _pointsModel;

    public void UpdateView(DynamicBuffer<CollectibleElement> collectibles)
    {
        if(collectibles.IsEmpty)
        {
            _pointsModel.PointsText.text = "Fire: 0, Water: 0, Earth: 0, Wind: 0";
            return;
        } else
        {
            _pointsModel.PointsText.text = BuildCollectibleString(collectibles).ToString();
        }
    }

    private FixedString512Bytes BuildCollectibleString(DynamicBuffer<CollectibleElement> buffer)
    {
        var result = new FixedString512Bytes();

        for (int i = 0; i < buffer.Length; i++)
        {
            var element = buffer[i];

            if (i > 0)
                result.Append(new FixedString32Bytes(", "));

            // Append the enum name
            switch (element.Type)
            {
                case CollectibleType.Fire: result.Append(new FixedString32Bytes("Fire")); break;
                case CollectibleType.Water: result.Append(new FixedString32Bytes("Water")); break;
                case CollectibleType.Earth: result.Append(new FixedString32Bytes("Earth")); break;
                case CollectibleType.Wind: result.Append(new FixedString32Bytes("Air")); break;
            }

            result.Append(new FixedString32Bytes(": "));
            result.Append(element.Amount);
        }

        return result;
    }

    private void Awake()
    {
        _pointsModel = GetComponent<CollectiblesModel>();
    }
}
