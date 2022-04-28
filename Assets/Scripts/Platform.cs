using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Platform : MonoBehaviour
{
    public Transform targetPoint;
    Vector3 startPos;
    [SerializeField] bool isStatic;
    void Start()
    {
        startPos = transform.position;

        if(!isStatic)
            StartCoroutine(MovementForward());
    }
    
    IEnumerator MovementForward()
    {
        transform.DOMove(targetPoint.position, 2);
        yield return new WaitForSeconds(6);
        StartCoroutine(MovementBack());
    }

    IEnumerator MovementBack()
    {
        transform.DOMove(startPos, 2);
        yield return new WaitForSeconds(6);
        StartCoroutine(MovementForward());
    }
}
