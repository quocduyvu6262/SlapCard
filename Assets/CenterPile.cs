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
            card.transform.SetParent(this.transform); 
            card.transform.localPosition = Vector3.zero;
            card.transform.localRotation = Quaternion.identity;

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
