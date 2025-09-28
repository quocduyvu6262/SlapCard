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
        // Get the object's name and remove "(Clone)" if it exists
        string objectName = gameObject.name.Replace("(Clone)", "");
        // Expecting prefab name like: "Card_Heart_Ace"
        string[] parts = objectName.Split('_');

        if (parts.Length == 3)
        {
            if (Enum.TryParse(parts[1], true, out CardSuit parsedSuit))
            {
                Suit = parsedSuit;
            }

            string rankString = parts[2];

            // Handle shorthand cases
            switch (rankString)
            {
                case "A": rankString = "Ace"; break;
                case "J": rankString = "Jack"; break;
                case "Q": rankString = "Queen"; break;
                case "K": rankString = "King"; break;
                case "2": rankString = "Two"; break;
                case "3": rankString = "Three"; break;
                case "4": rankString = "Four"; break;
                case "5": rankString = "Five"; break;
                case "6": rankString = "Six"; break;
                case "7": rankString = "Seven"; break;
                case "8": rankString = "Eight"; break;
                case "9": rankString = "Nine"; break;
                case "10": rankString = "Ten"; break;
            }

            if (Enum.TryParse(rankString, true, out CardRank parsedRank))
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

    public IEnumerator MoveCardSmooth(Vector3 start, Vector3 end, Quaternion startRot, Quaternion endRot, float duration)
    {
        float t = 0f;
        transform.position = start;
        transform.rotation = startRot;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(start, end, t);
            transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        transform.position = end;
        transform.rotation = endRot;
    }

    public void PrintCard()
    {
        Debug.Log($"Card: {Rank} of {Suit}");
    }
}
