using UnityEngine;

public class Card : MonoBehaviour
{
    public string suit;
    public string rank;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string[] parts = gameObject.name.Split('_');
        if (parts.Length == 3) {
            suit = parts[1];
            rank = parts[2];
        } else {
            Debug.LogWarning($"Card {gameObject.name} does not follow Rank_Suit naming!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrintCard()
    {
        Debug.Log($"Card: {rank} of {suit}");
    }
}
