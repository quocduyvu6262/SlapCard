using UnityEngine;
using System;

public enum CardSuit
{
    Club,
    Diamond,
    Heart,
    Spade
}

public enum CardRank
{
    Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
    Jack, Queen, King, Ace
}

public class Card : MonoBehaviour
{
    public CardSuit Suit { get; private set; }
    public CardRank Rank { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Expecting prefab name like: "Card_Heart_Ace"
        string[] parts = gameObject.name.Split('_');

        if (parts.Length == 3)
        {
            if (Enum.TryParse(parts[1], true, out CardSuit parsedSuit))
            {
                Suit = parsedSuit;
            }
            else
            {
                Debug.LogWarning($"Suit '{parts[1]}' not recognized for {gameObject.name}");
            }

            if (Enum.TryParse(parts[2], true, out CardRank parsedRank))
            {
                Rank = parsedRank;
            }
            else
            {
                Debug.LogWarning($"Rank '{parts[2]}' not recognized for {gameObject.name}");
            }
        }
        else
        {
            Debug.LogWarning($"Card {gameObject.name} does not follow Card_Suit_Rank naming!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrintCard()
    {
        Debug.Log($"Card: {Rank} of {Suit}");
    }
}
