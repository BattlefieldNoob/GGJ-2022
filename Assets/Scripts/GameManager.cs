using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public PhraseRecognitionManager PhraseRecognitionManager;
    public PhrasesRepository PhraseRepository;

    public float MentalSanity = 100;
    public float MentalSanityLostMultiplier = 2;

    public float GoodBadIndicator = 0;
    public int GoodBadThreshold = 10;


    public bool inGameplay = false;


    public readonly ISubject<PhraseScriptable> OnPhraseReachMouth = new Subject<PhraseScriptable>();

    private void Start()
    {
        StartGameplay();

        OnPhraseReachMouth.Subscribe((phrase) => { GoodBadIndicator += phrase.Multiplier * (phrase.IsGood ? 1 : -1); });
        OnPhraseReachMouth.Subscribe((phrase) =>
        {
            Debug.Log($"REACHED MOUTH:{phrase.Phrase}");
            PhraseRepository.CompletedPhrase(phrase);
        });
    }


    public void StartGameplay()
    {
        MentalSanity = 100;
        GoodBadIndicator = 0;
        inGameplay = true;
    }

    public void PlayerLose()
    {
        Debug.Log("PLAYER LOSE!!!!!!!!!!!!");
        inGameplay = false;
    }

    public void PlayerWin()
    {
        Debug.Log("PLAYER WIN!!!!!!!!!!!!");
        inGameplay = false;
    }


    private void Update()
    {
        if (!inGameplay) return;

        MentalSanity -= Time.deltaTime * MentalSanityLostMultiplier;
        if (MentalSanity <= 0)
        {
            PlayerLose();
        }

        if (Math.Abs(GoodBadIndicator) >= GoodBadThreshold)
        {
            PlayerWin();
        }
    }
}