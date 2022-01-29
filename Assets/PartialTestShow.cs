using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class PartialTestShow : MonoBehaviour
{

    public TextMeshProUGUI text;
    
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<GameManager>().PhraseRecognitionManager.PartialValidPhrase.Subscribe((phrase) =>
        {
            text.SetText(phrase);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
