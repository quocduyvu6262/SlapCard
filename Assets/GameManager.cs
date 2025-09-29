using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string cardFolderPath = "Asset_PlayingCards/Prefabs/Deck03";
    [Header("Deck Prefabs")]
    public GameObject[] cardPrefabs; // Drag all 52 card prefabs here
    private List<Card> deck = new List<Card>();
    public GameObject playerPrefab;
    public List<Player> players = new List<Player>();

    [Header("Player Setup")]
    public Transform tableCenter;

    void Awake()
    {
        cardPrefabs = Resources.LoadAll<GameObject>(cardFolderPath);
        playerPrefab = Resources.Load<GameObject>("PlayerPrefab");

        InitDeck();
        CreatePlayers();
    }

    void Start()
    {
        DealCards();

    }

    void Update()
    {

    }

    private void InitDeck()
    {
        deck.Clear();

        // Turn prefabs into card objects
        foreach (GameObject prefab in cardPrefabs)
        {
            GameObject cardObj = Instantiate(prefab);
            cardObj.name = prefab.name;
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
            new Vector3(1.5f, -5, -1),
            new Vector3(9f, 0, -1),
            new Vector3(1.5f, 5, -1),
            new Vector3(-6f, 0, -1)
        };

        Quaternion[] rotations = {
            Quaternion.identity,
            Quaternion.Euler(0,0,90),
            Quaternion.Euler(0,0,180),
            Quaternion.Euler(0,0,270)
        };

        string[] names = { "You", "den", "tamthe", "xiem" };
        bool[] isBot = { false, true, true, true };
        

        for (int i = 0; i < 4; i++)
        {
            // Instantiate prefab at desired position/rotation
            GameObject playerObj = Instantiate(playerPrefab, positions[i], rotations[i]);
            playerObj.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            Player playerComp = playerObj.GetComponent<Player>();

            // if (playerComp.name == "den")
            // {
            //     playerComp.defaultHand = playerComp.hand1d;
            //     playerComp.grabbingHand = playerComp.hand1g;
            // } else if (playerComp.name == "tamthe")
            // {
            //     playerComp.defaultHand = playerComp.hand2d;
            //     playerComp.grabbingHand = playerComp.hand2g;
            // } if (playerComp.name == "xiem")
            // {
            //     playerComp.defaultHand = playerComp.hand3d;
            //     playerComp.grabbingHand = playerComp.hand3g;
            // }

            // Set player data
            playerComp.Name = names[i];
            playerComp.IsBot = isBot[i];
            playerComp.BasePosition = positions[i];
            playerComp.BaseRotation = rotations[i];

            Transform uiAnchor = playerObj.transform.Find("UIAnchor");
            if (uiAnchor != null)
            {
                uiAnchor.localRotation = Quaternion.Inverse(playerObj.transform.rotation);
            }

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
            p.ArrangeHand();
            Debug.Log($"{p.Name} has {p.CardCount} cards");
        }
    }

    private IEnumerator DealCardsAnimated()
    {
        int cardsPerPlayer = 13;
        float dealDelay = 0.2f;

        for (int round = 0; round < cardsPerPlayer; round++)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (deck.Count == 0) yield break;
                Card card = deck[0];
                deck.RemoveAt(0);

                // Activate and start at table center
                card.gameObject.SetActive(true);
                card.transform.position = tableCenter.position;
                card.transform.localScale = new Vector3(0.2f, 0.2f, 1f);

                // Animate move to player
                yield return StartCoroutine(MoveCardToPlayer(card, players[i]));

                // Add card to player’s hand after it arrives
                players[i].AddCard(card);

                yield return new WaitForSeconds(dealDelay);
            }
        }

        // After dealing, arrange each player's hand visually
        foreach (Player p in players)
        {
            p.ArrangeHand();
            Debug.Log($"{p.Name} has {p.CardCount} cards");
        }
    }

    private IEnumerator MoveCardToPlayer(Card card, Player player)
    {
        Vector3 start = tableCenter.position;
        Vector3 end = player.BasePosition + new Vector3(0, 0.5f, 0); // small offset

        float t = 0f;
        float duration = 0.3f; // smooth move time

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            card.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
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
