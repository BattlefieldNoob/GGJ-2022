using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    public bool killed = false;
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    public Transform target;
    [SerializeField]
    public PhraseScriptable phrase;
    [SerializeField]
    public TMP_Text text;
    [SerializeField]
    private EnemiesManager manager;
    // Start is called before the first frame update
    void Start()
    {
        text.text = phrase.Phrase;
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
