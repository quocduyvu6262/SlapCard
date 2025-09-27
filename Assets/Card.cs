using UnityEngine;
using System;
using System.Collections;

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
    private bool isFaceUp = false;
    public CardSuit Suit { get; private set; }
    public CardRank Rank { get; private set; }

    private Quaternion faceUpRotation = Quaternion.Euler(0, 0, 0);
    private Quaternion faceDownRotation = Quaternion.Euler(0, 180, 0);


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

    // --- OPTION 1: INSTANT FLIP ---

    // Instantly flips the card to the target state
    public void FlipInstant()
    {
        isFaceUp = !isFaceUp;
        transform.localRotation = isFaceUp ? faceUpRotation : faceDownRotation;
    }

    // Instantly sets the card face up or down
    public void ShowFaceInstant(bool show)
    {
        isFaceUp = show;
        transform.localRotation = isFaceUp ? faceUpRotation : faceDownRotation;
    }


    // --- OPTION 2: ANIMATED FLIP (SMOOTHER) ---
    
    // Smoothly animates the card flip over a short duration
    public void FlipAnimated(float duration = 0.3f)
    {
        isFaceUp = !isFaceUp;
        StopAllCoroutines(); // Stop any existing flip animations
        StartCoroutine(FlipCoroutine(isFaceUp ? faceUpRotation : faceDownRotation, duration));
    }
    
    public void ShowFaceAnimated(bool show, float duration = 0.3f)
    {
        isFaceUp = show;
        StopAllCoroutines();
        StartCoroutine(FlipCoroutine(isFaceUp ? faceUpRotation : faceDownRotation, duration));
    }

    private IEnumerator FlipCoroutine(Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = transform.localRotation;
        float time = 0;

        while (time < duration)
        {
            // Slerp (Spherical Linear Interpolation) smoothly rotates between two Quaternions
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the final rotation is exact
        transform.localRotation = targetRotation;
    }

    public void PrintCard()
    {
        Debug.Log($"Card: {Rank} of {Suit}");
    }
}
