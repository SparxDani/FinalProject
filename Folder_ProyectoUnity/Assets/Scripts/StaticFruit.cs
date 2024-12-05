using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StaticFruit : Fruit
{
    [SerializeField] private float scaleMin = 0.8f;
    [SerializeField] private float scaleMax = 1.2f;
    [SerializeField] private float scaleDuration = 1f;
    [SerializeField] private float rotationSpeed = 360f;

    protected override void Start()
    {
        base.Start();
        ApplyTransformEffects();
    }

    private void ApplyTransformEffects()
    {
        transform.DOScale(scaleMax, scaleDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .From(scaleMin);

        RotateIndefinitely();
    }

    private void RotateIndefinitely()
    {
        StartCoroutine(RotateChildren());
    }

    private IEnumerator RotateChildren()
    {
        while (true)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
            yield return null;
        }
    }
}