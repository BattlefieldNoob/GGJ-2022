using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Random = System.Random;

public class PhrasesRepository : MonoBehaviourWithGameManager
{
    [SerializeField] private PhraseScriptable[] actualPhrases;

    private bool[] _inUseArray;

    private readonly Random rnd = new();

    private void Start()
    {
        _inUseArray = Enumerable.Repeat(false, actualPhrases.Length).ToArray();

        // Observable.Interval(TimeSpan.FromMilliseconds(1000)).Subscribe((_) =>
        // {
        //     gameManager.OnPhraseReachMouth.OnNext(GetPhrase());
        // });
    }

    public PhraseScriptable GetPhrase()
    {
        var notUsedPhrases = _inUseArray.Select((inUse, i) => (inUse, i)).Where(b => !b.inUse).ToArray();

        if (notUsedPhrases.Length == 0)
            return null;

        var (_, chosenIndex) = notUsedPhrases.OrderBy(_ => rnd.Next()).First();

        _inUseArray[chosenIndex] = true;
        PhraseScriptable phrase = actualPhrases[chosenIndex];
        gameManager.PhraseRecognitionManager.actualPhrases.Add(phrase);
        return phrase;
    }


    public void CompletedPhrase(PhraseScriptable phraseScriptable)
    {
        var index = Array.IndexOf(actualPhrases, phraseScriptable);
        if (index == -1)
            return;

        gameManager.PhraseRecognitionManager.actualPhrases.Remove(phraseScriptable);
        _inUseArray[index] = false;
    }
}