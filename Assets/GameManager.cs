using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string cardFolderPath = "Asset_PlayingCards/Prefabs/Deck01";
    [Header("Deck Prefabs")]
    public GameObject[] cardPrefabs; // Drag all 52 card prefabs here
    private List<Card> deck = new List<Card>();
    public GameObject playerPrefab;
    private List<Player> players = new List<Player>();

    [Header("Player Setup")]
    public Transform tableCenter;

    void Start()
    {
        cardPrefabs = Resources.LoadAll<GameObject>(cardFolderPath);
        playerPrefab = Resources.Load<GameObject>("PlayerPrefab");

        InitDeck();
        CreatePlayers();
        DealCards();
    }

    void Update() {
        
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
            new Vector3(0, -5, -1),
            new Vector3(9f, 0, -1),
            new Vector3(0, 5, -1),
            new Vector3(-9f, 0, -1)
        };

        Quaternion[] rotations = {
            Quaternion.identity,
            Quaternion.Euler(0,0,90),
            Quaternion.Euler(0,0,180),
            Quaternion.Euler(0,0,270)
        };

        string[] names = { "You", "Bot A", "Bot B", "Bot C" };
        bool[] isBot = { false, true, true, true };

        for (int i = 0; i < 4; i++)
        {
            // Instantiate prefab at desired position/rotation
            GameObject playerObj = Instantiate(playerPrefab, positions[i], rotations[i]);
            playerObj.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            Player playerComp = playerObj.GetComponent<Player>();

            // Set player data
            playerComp.Name = names[i];
            playerComp.IsBot = isBot[i];
            playerComp.BasePosition = positions[i];
            playerComp.BaseRotation = rotations[i];

            // Add to your list
            players.Add(playerComp);
        }

        Debug.Log("Players created and spawned in scene");
    }

    private void DealCards()
    {
        int cardsPerPlayer = 13; // 52 cards / 4 players
        int deckIndex = 0;

        for (int round = 0; round < cardsPerPlayer; round++)
        {
            foreach (Player player in players)
            {
                if (deckIndex >= deck.Count) break;

                Card card = deck[deckIndex++];
                player.AddCard(card);       // adds to queue & sets parent
            }
        }

        // After dealing, arrange each player's hand visually
        foreach (Player p in players)
        {
            Debug.Log("Here");
            p.ArrangeHand();
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
