using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOfTheDead : MonoBehaviour
{
    [SerializeField]
    private EnemiesManager manager;

    private void OnTriggerEnter(Collider other)
    {
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        manager.Complete(enemy);
    }
}
