using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public bool killed = false;
    public NavMeshAgent agent;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (killed == false)    
        {
            agent.SetDestination(target.position);
        } else
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }
}
