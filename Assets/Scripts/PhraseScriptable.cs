using UnityEngine;

[CreateAssetMenu(menuName = "GamePhrase")]
public class PhraseScriptable : ScriptableObject
{
    [Multiline]
    public string Phrase;
    public bool IsGood;
    public float Multiplier = 1;
}
