using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public string Name { get; set; }
    public bool IsBot { get; set; }
    public float botReactionTime = 1.0f;

    [Tooltip("The UI Text element for displaying the card count.")]
    public TextMeshProUGUI cardCountText;

    // Where the player sits in the scene
    public Vector3 BasePosition { get; set; }
    public Quaternion BaseRotation { get; set; }

    // Cards this player is holding (FIFO)
    private Queue<Card> hand;

    // For reaction timing (especially for bots)
    public float ReactionTimer { get; set; }

    // Sprite Renderer
    public SpriteRenderer handRenderer;  // Drag the HandSprite's SpriteRenderer here
    public Sprite defaultHand;
    public Sprite grabbingHand;


    void Awake()
    {
        hand = new Queue<Card>();
    }

    /// <summary>
    /// Updates the UI text to show the current number of cards in hand.
    /// </summary>
    public void UpdateCardCountUI()
    {
        if (cardCountText != null)
        {
            cardCountText.text = hand.Count.ToString();
        }
    }

    // ➕ Add a card to the player’s hand
    public void AddCard(Card card)
    {
        hand.Enqueue(card);
        card.gameObject.SetActive(true);
        card.transform.SetParent(this.transform);
        UpdateCardCountUI();
    }

    // Draw top card from hand
    public Card DrawCard()
    {
        if (hand.Count > 0)
        {
            Card drawnCard = hand.Dequeue();
            UpdateCardCountUI();
            return drawnCard;
        }
        return null;
    }

    public IEnumerator DrawCardWithAnimation(CenterPile centerPile, float grabDelay = 0.2f, float moveDuration = 0.3f)
    {
        yield return new WaitForSeconds(grabDelay);

        // Draw the card
        if (hand.Count == 0) yield break;
        Card card = hand.Dequeue();
        card.gameObject.SetActive(true);

        // Animate card to center pile
        yield return StartCoroutine(card.MoveCardSmooth(
            BasePosition,
            centerPile.transform.position + new Vector3(0, 0.5f, 0),
            card.transform.rotation,
            Quaternion.identity,
            moveDuration
        ));

        centerPile.AddCard(card);
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
        Vector3 handOffset = Vector3.zero;
        handOffset = new Vector3(-3f, 2f, 0);

        int i = 0;
        // A very small Z offset for each card to create a 3D stack and prevent visual flickering (Z-fighting).
        float zOffsetPerCard = -0.01f;

        foreach (Card card in hand)
        {
            card.transform.SetParent(this.transform);
            card.transform.localPosition = handOffset + new Vector3(i * (-0.02f), 0, i * zOffsetPerCard);
            card.transform.localRotation = Quaternion.identity;
            card.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            card.ShowFaceInstant(false);
            i++;
        }
    }

    //Adds a list of cards to the bottom of the player's hand.
    public void AddCardsToHand(List<Card> cardsToAdd)
    {
        foreach (Card card in cardsToAdd)
        {
            // Enqueue adds the card to the end of the line (the bottom of the deck).
            hand.Enqueue(card);

            // Update the card's parent to belong to this player
            card.transform.SetParent(this.transform);
        }

        // After adding cards, it's good practice to tidy up the visual stack
        ArrangeHand();
        UpdateCardCountUI();
    }

    bool SameRotation(Quaternion a, Quaternion b)
    {
        return Quaternion.Dot(a, b) > 0.9999f; // threshold for tolerance
    }

    public void SetHandGrabbing(bool isGrabbing)
    {
        if (handRenderer != null)
        {
            handRenderer.sprite = isGrabbing ? grabbingHand : defaultHand;
        }
    }

    public IEnumerator AnimateSlap(CenterPile centerPile, float grabDuration = 0.2f, float handReachDistance = 1f)
    {
        if (handRenderer == null || centerPile.IsEmpty) yield break;

        Vector3 startPos = handRenderer.transform.localPosition;

        // Calculate a direction toward the center pile
        Vector3 direction = (centerPile.transform.position - transform.position).normalized;
        Vector3 endPos = startPos + new Vector3(direction.x, direction.y, 0) * handReachDistance * 5f;

        float t = 0f;
        SetHandGrabbing(true);

        // Move hand toward center
        while (t < 1f)
        {
            t += Time.deltaTime / grabDuration;
            handRenderer.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // Wait briefly at center (like grabbing)
        yield return new WaitForSeconds(0.1f);

        // Move hand back
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / grabDuration;
            handRenderer.transform.localPosition = Vector3.Lerp(endPos, startPos, t);
            yield return null;
        }

        SetHandGrabbing(false);
    }

    // Number of cards left
    public int CardCount => hand.Count;

    // Is this player still in the game?
    public bool HasCards => hand.Count > 0;
}
