using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string cardFolderPath = "Asset_PlayingCards/Prefabs/Deck01";
    [Header("Deck Prefabs")]
    public GameObject[] cardPrefabs; // Drag all 52 card prefabs here
    private List<Card> deck = new List<Card>();
    private List<Player> players = new List<Player>();

    [Header("Player Setup")]
    public Transform tableCenter;

    void Start()
    {
        cardPrefabs = Resources.LoadAll<GameObject>(cardFolderPath);
        InitDeck();
        CreatePlayers();
        DealCards();
    }

    private void InitDeck()
    {
        deck.Clear();

        // Turn prefabs into card objects
        foreach (GameObject prefab in cardPrefabs)
        {
            GameObject cardObj = Instantiate(prefab, this.transform);
            Card card = cardObj.GetComponent<Card>();

            if (card != null)
            {
                deck.Add(card);
                cardObj.SetActive(false); // Hide until played
            }
        }

        Shuffle(deck);
        Debug.Log("Deck initialized with " + deck.Count + " cards");
    }

    private void CreatePlayers()
    {
        players.Clear();

        // Positions around table (top, right, bottom, left)
        Vector3[] positions = {
            new Vector3(0, -4, 0),
            new Vector3(6, 0, 0),
            new Vector3(0, 4, 0),
            new Vector3(-6, 0, 0)
        };

        Quaternion[] rotations = {
            Quaternion.identity,
            Quaternion.Euler(0,0,90),
            Quaternion.Euler(0,0,180),
            Quaternion.Euler(0,0,270)
        };

        players.Add(new Player("You", false, positions[0], rotations[0]));
        players.Add(new Player("Bot A", true, positions[1], rotations[1]));
        players.Add(new Player("Bot B", true, positions[2], rotations[2]));
        players.Add(new Player("Bot C", true, positions[3], rotations[3]));

        Debug.Log("Players created");
    }

    private void DealCards()
    {
        int playerIndex = 0;
        foreach (Card card in deck)
        {
            players[playerIndex].AddCard(card);
            playerIndex = (playerIndex + 1) % players.Count;
        }

        foreach (Player p in players)
        {
            Debug.Log($"{p.Name} has {p.CardCount} cards");
        }
    }

    // Simple shuffle
    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
