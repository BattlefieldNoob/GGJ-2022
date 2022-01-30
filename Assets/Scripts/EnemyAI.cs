using System.Collections;
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

    [SerializeField] private AudioClip poppingClip;
    [SerializeField] private AudioClip voice;

    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        text.text = phrase.Phrase;
        _audioSource = GetComponentInChildren<AudioSource>();
        _audioSource.clip = voice;
        _audioSource.loop = true;
        _audioSource.pitch = Random.Range(-3, 1.5f);
        _audioSource.Play();
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
    public void Die()
    {
        killed = true;
        _audioSource.PlayOneShot(poppingClip);
        // animate;
        StartCoroutine(Unspawn());
    }

    public IEnumerator Unspawn()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
