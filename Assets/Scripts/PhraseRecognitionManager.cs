using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class PhraseRecognitionManager : MonoBehaviourWithGameManager
{
    private List<KeyCode> _keyCodes = new List<KeyCode>();

    public IObservable<string> KeyObservable;

    public ISubject<PhraseScriptable> ValidPhrase = new Subject<PhraseScriptable>();
    public ISubject<string> PartialValidPhrase = new Subject<string>();
    public ISubject<PhraseScriptable> WrongPhrase = new Subject<PhraseScriptable>();

    public List<PhraseScriptable> actualPhrases = new List<PhraseScriptable>();

    public string partialPhrase;

    // Start is called before the first frame update
    void Start()
    {
        _keyCodes = Enumerable.Range(97, 26).Select(i => (KeyCode)i).ToList();

        var anyKeyObservable = Observable.EveryUpdate()
            .Where(_ => Input.anyKeyDown);

        KeyObservable = anyKeyObservable.SelectMany(_ => _keyCodes)
            .Where(Input.GetKeyDown)
            .Select(code => code.ToString());

        // var mytest = "ciao come stai ciaa ciao come stai".RemoveWhitespace();
        // 
        // KeyObservable = Observable.Interval(TimeSpan.FromMilliseconds(250))
        //     .Select((l, i) => i < mytest.Length ? mytest[i].ToString() : "9");

        KeyObservable.Subscribe((letter) => { Debug.Log(letter); });


        // phrase recognition


        int currentIndex = 0;
        var valid = new List<PhraseScriptable>();
        var blocked = new List<PhraseScriptable>();

        KeyObservable.Subscribe((currentLetter) =>
        {
            foreach (var phraseScriptable in actualPhrases)
            {
                var phrase = phraseScriptable.Phrase.RemoveWhitespace().ToLower();

                if (phrase.Length < currentIndex)
                {
                    if (!valid.Contains(phraseScriptable))
                        valid.Add(phraseScriptable);
                }
                else
                {
                    if (!blocked.Contains(phraseScriptable) && phrase[currentIndex] == currentLetter.ToLower().ToCharArray().FirstOrDefault())
                    {
                        if (phrase.Length - 1 == currentIndex)
                        {
                            ValidPhrase.OnNext(phraseScriptable);
                            currentIndex = 0;
                            valid.Clear();
                            blocked.Clear();
                            partialPhrase = "";
                            break;
                        }

                        if (!valid.Contains(phraseScriptable))
                            valid.Add(phraseScriptable);
                    }
                    else
                    {
                        if (valid.Contains(phraseScriptable))
                        {
                            valid.Remove(phraseScriptable);
                            WrongPhrase.OnNext(phraseScriptable);
                            blocked.Add(phraseScriptable);
                        }
                    }
                }
            }

            currentIndex++;

            if (valid.Count == 0)
            {
                currentIndex = 0;
                blocked.Clear();
                partialPhrase = "";
            }
            else
            {
                partialPhrase += currentLetter.ToLower();
                PartialValidPhrase.OnNext(partialPhrase);
            }
        });

        ValidPhrase.Subscribe((valid) => { Debug.Log($"VALID!:{valid.Phrase}"); });
        WrongPhrase.Subscribe((wrong) => { Debug.Log($"REMOVED:{wrong.Phrase}"); });
        PartialValidPhrase.Subscribe((s) => { Debug.Log($"Partial:{s}"); });
    }
}