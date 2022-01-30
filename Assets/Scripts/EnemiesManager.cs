using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.Playables;
using Random = UnityEngine.Random;


public class EnemiesManager : MonoBehaviourWithGameManager
{
    [SerializeField] private List<EnemyAI> enemies = new List<EnemyAI>();
    [SerializeField] private EnemyAI enemy;
    [SerializeField] private float spawnTime = 2f;
    [SerializeField] private Transform spawnOrigin;
    [SerializeField] private float range = 5;
    [SerializeField] private Transform exit;
    [SerializeField] private PhraseScriptable testPhrase;
    [SerializeField] private Color devilColor;
    [SerializeField] private Color angelColor;

    [SerializeField] private RuntimeAnimatorController[] Devil;

    [SerializeField] private RuntimeAnimatorController[] Angel;

    private IDisposable subscription;


    private readonly System.Random rnd = new();


    // Start is called before the first frame update
    void Start()
    {
        gameManager.PhraseRecognitionManager.ValidPhrase.Subscribe((writtenPhrase) =>
        {
            EnemyAI findedEnemy = enemies.Find((enemy) => { return enemy.phrase == writtenPhrase; });
            RemoveEnemy(enemy);
            findedEnemy.Die();
        });
        subscription = Observable.Interval(TimeSpan.FromSeconds(spawnTime)).Subscribe((_) => { Spawn(); });
    }

    void Spawn()
    {
        float locationX = Random.Range(spawnOrigin.position.x - range, spawnOrigin.position.x + range);
        Vector3 position = new Vector3(locationX, spawnOrigin.position.y, spawnOrigin.position.z);

        EnemyAI newEnemy = Instantiate(enemy, position, spawnOrigin.rotation);
        ConfigureEnemy(newEnemy);
        enemies.Add(newEnemy);
    }

    private void ConfigureEnemy(EnemyAI newEnemy)
    {
        newEnemy.phrase = gameManager.PhraseRepository.GetPhrase();
        // newEnemy.phrase = testPhrase;
        newEnemy.textColor = newEnemy.phrase.IsGood ? angelColor : devilColor;
        newEnemy.target = exit;
        var enemyAnimator = newEnemy.GetComponentInChildren<Animator>();

        enemyAnimator.runtimeAnimatorController = newEnemy.phrase.IsGood
            ? Angel.OrderBy(_ => rnd.Next()).First()
            : Devil.OrderBy(_ => rnd.Next()).First();

        if (newEnemy.transform.position.x < spawnOrigin.position.x)
        {
            newEnemy.GetComponentInChildren<SpriteRenderer>().flipX = true;
        }
    }


    public void Annichilation()
    {
        subscription.Dispose();
        enemies.ForEach((enemy) =>
        {
            StartCoroutine(enemy.Unspawn());
        });
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
        StartCoroutine(enemy.Unspawn());
    }
}