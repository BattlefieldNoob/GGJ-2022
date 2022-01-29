using System;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class PhrasesRepository : MonoBehaviourWithGameManager
{
    [SerializeField]
    private PhraseScriptable[] actualPhrases;

    private bool[] _inUseArray;

    private readonly Random rnd = new ();

    private void Start()
    {
        _inUseArray = Enumerable.Repeat(false, actualPhrases.Length).ToArray();
    }

    public PhraseScriptable GetPhrase()
    {
        var chosen = _inUseArray.Select((inUse, i) => (inUse, i)).Where(b => !b.inUse).OrderBy(_ => rnd.Next())
            .FirstOrDefault();

        if (chosen == default)
            return null;

        _inUseArray[chosen.i] = true;
        return actualPhrases[chosen.i];
    }


    public void CompletedPhrase(PhraseScriptable phraseScriptable)
    {
        var index = Array.IndexOf(actualPhrases, phraseScriptable);
        if (index == -1)
            return;

        _inUseArray[index] = false;
    }
}