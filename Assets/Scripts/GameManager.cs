using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public PhraseRecognitionManager PhraseRecognitionManager;
    public PhrasesRepository PhraseRepository;
    public EnemiesManager EnemiesManager;
    [SerializeField]
    private Image goodMetre;
    [SerializeField]
    private Image evilMetre;

    public float MentalSanity = 100;
    public float MentalSanityLostMultiplier = 2;

    public float GoodLevel = 0;
    public float EvilLevel= 0;
    public int GoodBadThreshold = 10;


    public bool inGameplay = false;


    public readonly ISubject<PhraseScriptable> OnPhraseReachMouth = new Subject<PhraseScriptable>();

    private void Start()
    {
        StartGameplay();

        OnPhraseReachMouth.Subscribe((phrase) => {
            if (phrase.IsGood)
            {
                GoodLevel += phrase.Multiplier * 1;
                goodMetre.fillAmount = GoodLevel / GoodBadThreshold;
            } else
            {
                EvilLevel += phrase.Multiplier * 1;
                evilMetre.fillAmount = EvilLevel / GoodBadThreshold;
            }
        });
        // OnPhraseReachMouth.Subscribe((phrase) =>
        // {
        //     Debug.Log($"REACHED MOUTH:{phrase.Phrase}");
        //     PhraseRepository.CompletedPhrase(phrase);
        // });
    }


    public void StartGameplay()
    {
        goodMetre.fillAmount = 0;
        evilMetre.fillAmount = 0;
        MentalSanity = 100; 
        GoodLevel = 0;
        EvilLevel = 0;
        inGameplay = true;
    }

    public void PlayerLose()
    {
        inGameplay = false;
        Debug.Log("PLAYER LOSE!!!!!!!!!!!!");
        PhraseRecognitionManager.StopListen();
        EnemiesManager.Annichilation();
    }

    public void PlayerWin()
    {
        inGameplay = false;
        Debug.Log("PLAYER WIN!!!!!!!!!!!!");
        if (GoodLevel >= GoodBadThreshold)
        {
            Debug.Log("GOOD ENDING !!!!!!!!!!!!");
        } else if (EvilLevel >= GoodBadThreshold)
        {
            Debug.Log("BAD ENDING !!!!!!!!!!!!");
        }
        PhraseRecognitionManager.StopListen();
        EnemiesManager.Annichilation();
        
    }


    private void Update()
    {
        if (!inGameplay) return;

        MentalSanity -= Time.deltaTime * MentalSanityLostMultiplier;
        if (MentalSanity <= 0)
        {
            PlayerLose();
        }

        if (EvilLevel >= GoodBadThreshold || GoodLevel >= GoodBadThreshold)
        {
            PlayerWin();
        }
    }
}