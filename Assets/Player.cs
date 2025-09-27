using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string Name { get; private set; }
    public bool IsBot { get; private set; }

    // Where the player sits in the scene
    public Vector3 BasePosition { get; set; }
    public Quaternion BaseRotation { get; set; }

    // Cards this player is holding (FIFO)
    private readonly Queue<Card> hand = new Queue<Card>();

    // For reaction timing (especially for bots)
    public float ReactionTimer { get; set; }

    public Player(string name, bool isBot, Vector3 position, Quaternion rotation)
    {
        Name = name;
        IsBot = isBot;
        BasePosition = position;
        BaseRotation = rotation;
    }

    // ➕ Add a card to the player’s hand
    public void AddCard(Card card)
    {
        if (card != null)
            hand.Enqueue(card);
    }

    // Draw top card from hand
    public Card DrawCard()
    {
        return hand.Count > 0 ? hand.Dequeue() : null;
    }

    // Peek at top card (no removal)
    public Card PeekCard()
    {
        return hand.Count > 0 ? hand.Peek() : null;
    }

    // Slap attempt (target is usually the center pile)
    public bool TrySlap(CenterPile pile)
    {
        if (pile == null || pile.IsEmpty) return false;

        var topCard = pile.PeekTopCard();
        if (topCard != null && topCard.Rank == CardRank.Jack)
        {
            Debug.Log($"{Name} slapped correctly!");
            return true;
        }

        Debug.Log($"{Name} slapped incorrectly!");
        return false;
    }

    // Number of cards left
    public int CardCount => hand.Count;

    // Is this player still in the game?
    public bool HasCards => hand.Count > 0;
}
