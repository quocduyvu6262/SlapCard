using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour
{
    public string Name { get; set; }
    public bool IsBot { get; set; }

    // Where the player sits in the scene
    public Vector3 BasePosition { get; set; }
    public Quaternion BaseRotation { get; set; }

    // Cards this player is holding (FIFO)
    private Queue<Card> hand;

    // For reaction timing (especially for bots)
    public float ReactionTimer { get; set; }

    void Awake()
    {
        hand = new Queue<Card>();
    }

    // ➕ Add a card to the player’s hand
    public void AddCard(Card card)
    {
        hand.Enqueue(card);
        card.gameObject.SetActive(true);
        card.transform.SetParent(this.transform);
    }

    // Draw top card from hand
    public Card DrawCard()
    {
        if (hand.Count > 0)
            return hand.Dequeue();
        return null;
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

    public void ArrangeHand()
    {
        int i = 0;
        float spacing = 0.8f;

        Vector3 offset = Vector3.zero;
        if (BasePosition.y > 0) offset = new Vector3(0, -0.5f, 0);
        if (BasePosition.y < 0) offset = new Vector3(0, 0.5f, 0);
        if (BasePosition.x > 0) offset = new Vector3(-0.5f, 0, 0);
        if (BasePosition.x < 0) offset = new Vector3(0.5f, 0, 0);

        foreach (Card card in hand)
        {
            card.transform.localPosition = offset + new Vector3(i * spacing, 0, 0);
            card.transform.localRotation = Quaternion.identity;
            card.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            i++;
        }
    }

    // Number of cards left
    public int CardCount => hand.Count;

    // Is this player still in the game?
    public bool HasCards => hand.Count > 0;
}
