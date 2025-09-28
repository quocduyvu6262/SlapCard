using System.Collections.Generic;
using UnityEngine;

public class CenterPile: MonoBehaviour
{
    public List<Card> pile;
    private bool positionLeft = true;

    void Awake()
    {
        pile = new List<Card>();
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
                Vector3 pos = topCard.transform.position;
                pos.z = -2f;
                topCard.transform.position = pos;

                if (pile.Count >= 3) {
                    Card thirdCard = pile[pile.Count - 3];
                    thirdCard.transform.position = new Vector3(0f,0f,-1f);
                }
            }

            if (positionLeft){
                card.transform.position = new Vector3(-0.5f,0f,-3f);
            } else {
                card.transform.position = new Vector3(0f,0f,-3f);
            }

            positionLeft = !positionLeft;

            card.transform.localScale = originalScale;

            pile.Add(card);
            Debug.Log($"Added {card} to center pile");
        }
    }

    // Look at the top card without removing
    public Card PeekTopCard()
    {
       Card topCard = pile.Count > 0 ? pile[pile.Count - 1] : null;
       return topCard;
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
