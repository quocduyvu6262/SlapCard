using UnityEngine;
using System.Collections.Generic;

public class SlapCard : MonoBehaviour
{   
    public static SlapCard instance;

    // -- Card Setup -- 
    public string cardFolderPath = "Asset_PlayingCards/Prefabs/Deck01";  // under Resources/
    [Header("Deck Prefabs")]
    [Tooltip("Drop all 52 card prefabs here.")]
    public GameObject[] cardPrefabs;
    private List<GameObject> spawnedCards = new List<GameObject>();

    // --- Added Grid Settings ---
    [Header("Grid Settings")]
    [Tooltip("How many columns the grid should have.")]
    public int columns = 13;
    [Tooltip("The horizontal and vertical space between each card.")]
    public Vector2 spacing = new Vector2(0.02f, 0.02f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardPrefabs = Resources.LoadAll<GameObject>(cardFolderPath);
        SpawnDeck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnDeck()
    {
        // Clean up old cards
        foreach (var card in spawnedCards)
        {
            Destroy(card);
        }
        spawnedCards.Clear();

        // Shuffle the prefabs
        List<GameObject> shuffled = new List<GameObject>(cardPrefabs);
        Shuffle(shuffled);

        // Instantiate cards
        foreach (GameObject prefab in shuffled)
        {
            GameObject card = Instantiate(prefab, this.transform);
            spawnedCards.Add(card);
        }

        ArrangeCards();
    }

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

    [ContextMenu("Arrange Cards in Grid")]
    public void ArrangeCards()
    {
        Card[] cardsToArrange = transform.GetComponentsInChildren<Card>();
        Debug.Log(cardsToArrange);  
        if (cardsToArrange.Length == 0) return;

        // Center the grid
        float gridWidth = (columns - 1) * spacing.x;
        int numRows = Mathf.CeilToInt((float)cardsToArrange.Length / columns);
        float gridHeight = (numRows - 1) * spacing.y;
        Vector3 startOffset = new Vector3(-gridWidth / 2f, gridHeight / 2f, 0);

        // Position each card
        for (int i = 0; i < cardsToArrange.Length; i++)
        {
            int col = i % columns;
            int row = i / columns;
            float xPos = col * spacing.x;
            float yPos = -row * spacing.y;

            float screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
            float cardWidth = 1f; // original prefab width
            float scaleFactor = (screenWidth / columns) / cardWidth * 0.03f;

            Debug.Log($"Card {i}: xPos = {xPos}, yPos = {yPos}");
            
            cardsToArrange[i].transform.localPosition = new Vector3(xPos, yPos, -1) + startOffset;
            cardsToArrange[i].transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        }
    }

}
