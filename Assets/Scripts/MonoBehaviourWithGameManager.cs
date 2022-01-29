using UnityEngine;

public class MonoBehaviourWithGameManager : MonoBehaviour
{
    protected GameManager gameManager;
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
}
