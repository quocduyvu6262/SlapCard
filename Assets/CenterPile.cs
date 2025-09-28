using System.Collections.Generic;
using UnityEngine;

public class CenterPile: MonoBehaviour
{
    public Stack<Card> pile;

    void Awake()
    {
        pile = new Stack<Card>();
    }

    // Add a card to the pile
    public void AddCard(Card card)
    {
        if (card != null)
        {
            card.gameObject.SetActive(true);

            Vector3 originalScale = card.transform.localScale;

            card.transform.position = this.transform.position;
            card.transform.rotation = this.transform.rotation;
            Card topCard = PeekTopCard();
            if (topCard) {
                Vector3 pos = card.transform.position;
                pos.z = -1f;
                topCard.transform.position = pos;
            }
            card.transform.position = new Vector3(0f,0f,-2f);
            card.transform.localScale = originalScale;

            pile.Push(card);
            Debug.Log($"Added {card} to center pile");
        }
    }

    // Look at the top card without removing
    public Card PeekTopCard()
    {
        return pile.Count > 0 ? pile.Peek() : null;
    }

    // Take all cards (e.g., when someone wins a slap)
    public List<Card> TakeAllCards()
    {
        List<Card> cards = new List<Card>(pile);
        pile.Clear();
        return cards;
    }

    // Is pile empty?
    public bool IsEmpty => pile.Count == 0;
}
