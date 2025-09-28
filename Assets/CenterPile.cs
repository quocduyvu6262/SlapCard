using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CenterPile : MonoBehaviour
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

    public void AddCardToBottom(Card card)
    {
        if (card == null) return;

        // 1. Convert the stack to a temporary list
        List<Card> tempList = pile.ToList();

        // 2. Add the new card to the beginning of the list
        tempList.Insert(0, card);

        // 3. Clear the original stack
        pile.Clear();

        // 4. Re-populate the stack from the modified list
        // We must loop backwards to maintain the correct stack order
        for (int i = tempList.Count - 1; i >= 0; i--)
        {
            pile.Push(tempList[i]);
        }

        Debug.Log($"Added {card.name} to the bottom of the center pile.");
    }

    // Is pile empty?
    public bool IsEmpty => pile.Count == 0;
}
