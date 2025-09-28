using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    private GameManager gameManager;
    private List<Player> players;
    private CenterPile centerPile;
    private List<Card> pile;

    [Tooltip("The time in seconds between each card being played.")]
    public float playSpeed = 1f;


    // reference the script
    void Awake()
    {
        gameManager = GetComponent<GameManager>();
        centerPile = GetComponent<CenterPile>();
        players = gameManager.players;
        pile = centerPile.pile;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(players.Count);
        Debug.Log(pile.Count);
        StartCoroutine(PlayCardsRoutine());
    }

    /// <summary>
    /// A continuous loop that has each player play a card in order.
    /// </summary>
    private IEnumerator PlayCardsRoutine()
    {
        // This loop will run forever until the game ends
        while (true) 
        {
            // Go through each player
            foreach (Player currentPlayer in players)
            {
                // Only play a card if the player still has some
                if (currentPlayer.HasCards)
                {
                    Card cardToPlay = currentPlayer.DrawCard();
                    centerPile.AddCard(cardToPlay);

                    // You could check for a slap condition here automatically if you wanted
                    // if(CheckSlapCondition()) { ... }
                }

                // Wait for a moment before the next player goes
                yield return new WaitForSeconds(playSpeed);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
