using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.VisualScripting;
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
    private List<PhraseScriptable> _valid;

    // Start is called before the first frame update
    void Start()
    {
        _keyCodes = Enumerable.Range(97, 26).Select(i => (KeyCode)i).ToList();

        _keyCodes.AddRange(Enumerable.Range(48, 10).Select(i => (KeyCode)i));

        _keyCodes.Add(KeyCode.Space);

        var anyKeyObservable = Observable.EveryUpdate()
            .Where(_ => Input.anyKeyDown);

        KeyObservable = anyKeyObservable.SelectMany(_ => _keyCodes)
            .Where(Input.GetKeyDown)
            .Select(code => code.ToString().Replace("Alpha", ""));

        // var mytest = "ciao come stai ciaa ciao come stai".RemoveWhitespace();
        // 
        // KeyObservable = Observable.Interval(TimeSpan.FromMilliseconds(250))
        //     .Select((l, i) => i < mytest.Length ? mytest[i].ToString() : "9");

        KeyObservable.Subscribe((letter) => { Debug.Log(letter); });


        // phrase recognition


        int currentIndex = 0;
        _valid = new List<PhraseScriptable>();
        var blocked = new List<PhraseScriptable>();

        anyKeyObservable.Where(_ => Input.GetKeyDown(KeyCode.Return)).Subscribe((_) =>
        {
            Debug.Log("ENTER!");

            var phrase = _valid.FirstOrDefault((p) =>
                p.Phrase.RemoveWhitespace().ToLower() == partialPhrase.RemoveWhitespace());
            if (phrase != null)
            {
                ValidPhrase.OnNext(phrase);
            }

            //if (partialPhrase.Length != 0)
            //{
            //    var found = actualPhrases.Where(scriptable => scriptable != null && scriptable.Phrase != null)
            //        .Select((p, i) => (p.Phrase.RemoveWhitespace().ToLower(), i))
            //        .FirstOrDefault(phrase => phrase.Item1 == partialPhrase.RemoveWhitespace());
//
            //    Debug.Log(found);
            //    if (found != default)
            //    {
            //        ValidPhrase.OnNext(actualPhrases[found.i]);
            //    }
            //}

            currentIndex = 0;
            blocked.Clear();
            _valid.Clear();
            partialPhrase = "";
            PartialValidPhrase.OnNext(partialPhrase);
        });


        KeyObservable.Subscribe((currentLetter) =>
        {
            if (currentLetter == "Space" && partialPhrase.Length != 0)
            {
                //search for corrent phrase

                partialPhrase += " ";
                PartialValidPhrase.OnNext(partialPhrase);
                return;
            }

            foreach (var phraseScriptable in actualPhrases)
            {
                if (phraseScriptable == null)
                    continue;

                if (phraseScriptable.Phrase == null)
                    continue;

                var phrase = phraseScriptable.Phrase.RemoveWhitespace().ToLower();

                if (phrase.Length < currentIndex)
                {
                    if (!_valid.Contains(phraseScriptable))
                        _valid.Add(phraseScriptable);
                }
                else
                {
                    if (!blocked.Contains(phraseScriptable) && currentIndex < phrase.Length && phrase[currentIndex] ==
                        currentLetter.ToLower().ToCharArray().FirstOrDefault())
                    {
                        //if (phrase.Length - 1 == currentIndex)
                        //{
                        //    ValidPhrase.OnNext(phraseScriptable);
                        //    currentIndex = 0;
                        //    valid.Clear();
                        //    blocked.Clear();
                        //    partialPhrase = "";
                        //    break;
                        //}

                        if (!_valid.Contains(phraseScriptable))
                            _valid.Add(phraseScriptable);
                    }
                    else
                    {
                        if (_valid.Contains(phraseScriptable))
                        {
                            _valid.Remove(phraseScriptable);
                            WrongPhrase.OnNext(phraseScriptable);
                            blocked.Add(phraseScriptable);
                        }
                    }
                }
            }

            currentIndex++;

            if (_valid.Count == 0)
            {
                currentIndex = 0;
                blocked.Clear();
                partialPhrase = "";
                PartialValidPhrase.OnNext(partialPhrase);
            }
            else
            {
                partialPhrase += currentLetter.ToLower();
                PartialValidPhrase.OnNext(partialPhrase);
            }
        });

        ValidPhrase.Subscribe((valid) => { Debug.Log($"VALID:{valid.Phrase}"); });
        WrongPhrase.Subscribe((wrong) => { Debug.Log($"NOT POSSIBLE:{wrong.Phrase}"); });
        PartialValidPhrase.Subscribe((s) => { Debug.Log($"Partial:{s}"); });
    }
}