using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ۼ��� : ������
/// �����ۼ��� : 2022/04/04
/// ���������� : 
/// ���� : 
/// 
/// �߰ݽ��������� �߰���
/// </summary>

public class Tracer : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float delayTime = 2f;
    Transform tr;
    private void Awake()
    {
        tr = transform;
    }

    public void StartMove()
    {
        StartCoroutine(E_Move());
    }

    IEnumerator E_Move()
    {
        yield return new WaitForSeconds(delayTime);
        while (true)
        {
            tr.Translate(Vector3.up * speed * Time.deltaTime);
            yield return null;
        }
    }
}