using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Random = UnityEngine.Random;


public class EnemiesManager : MonoBehaviourWithGameManager
{
    [SerializeField]
    private List<EnemyAI> enemies = new List<EnemyAI>();
    [SerializeField]
    private EnemyAI enemy;
    [SerializeField]
    private float spawnTime = 2f;
    [SerializeField]
    private Transform spawnOrigin;
    [SerializeField]
    private float range = 5;
    [SerializeField]
    private Transform exit;
    [SerializeField]
    private PhraseScriptable testPhrase;
    
    private IDisposable subscription;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.PhraseRecognitionManager.ValidPhrase.Subscribe((writtenPhrase) => {
            EnemyAI findedEnemy =  enemies.Find((enemy) => {
                return enemy.phrase == writtenPhrase;
            });
            RemoveEnemy(enemy);
            findedEnemy.Die();
        });
        subscription = Observable.Interval(TimeSpan.FromSeconds(spawnTime)).Subscribe((_) => {
            Spawn();
        });
    }

    void Spawn()
    {
        float locationX = Random.Range(spawnOrigin.position.x - range, spawnOrigin.position.x + range);
        Vector3 position = new Vector3(locationX, spawnOrigin.position.y, spawnOrigin.position.z);
        
        EnemyAI newEnemy = Instantiate(enemy, position, spawnOrigin.rotation);
        newEnemy.phrase = gameManager.PhraseRepository.GetPhrase();
        // newEnemy.phrase = testPhrase;
        newEnemy.target = exit;
        enemies.Add(newEnemy);
    }

    public void Annichilation()
    {
        subscription.Dispose();
        enemies.ForEach((enemy) => { enemy.Unspawn(); });
    }

    private void RemoveEnemy(EnemyAI enemy)
    {
        gameManager.PhraseRepository.CompletedPhrase(enemy.phrase);
        enemies.Remove(enemy);
    }

    public void Complete(EnemyAI enemy)
    {
        gameManager.OnPhraseReachMouth.OnNext(enemy.phrase);
        RemoveEnemy(enemy);
        enemy.Unspawn();
    }
}
