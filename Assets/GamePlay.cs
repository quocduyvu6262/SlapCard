using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePlay : MonoBehaviour
{
    private GameManager gameManager;
    private List<Player> players;
    private CenterPile centerPile;
    private List<Card> pile;
    private bool pileClaimed = false;

    private int activeBotSlaps = 0;

    [Tooltip("The time in seconds between each card being played.")]
    public float playSpeed = 0.5f;

    [Header("Audio")]
    public AudioClip cardPlaySound;
    public AudioClip slapSound;
    private AudioSource audioSource;

    private HashSet<Player> playersWithNoCards = new HashSet<Player>();

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
        centerPile = GetComponent<CenterPile>();
        players = gameManager.players;
        pile = centerPile.pile;
        audioSource = GetComponent<AudioSource>();
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
                if (currentPlayer.CardCount == 52)
                {
                    Debug.Log("Game finished, player: " + currentPlayer.name + " got all 52 cards");
                    yield break;

                }
                // Only play a card if the player still has some
                if (currentPlayer.HasCards)
                {
                    // Card cardToPlay = currentPlayer.DrawCard();
                    // centerPile.AddCard(cardToPlay);
                    yield return StartCoroutine(currentPlayer.DrawCardWithAnimation(centerPile));
                    pileClaimed = false;
                    if (cardPlaySound != null)
                    {
                        audioSource.PlayOneShot(cardPlaySound);
                    }

                    // You could check for a slap condition here automatically if you wanted
                    // if(CheckSlapCondition()) { ... }
                    if (CheckForSlapCondition())
                    {
                        TriggerBotSlaps();
                        yield return new WaitUntil(() => activeBotSlaps == 0);
                    }

                    if (!currentPlayer.HasCards)
                    {
                        playersWithNoCards.Add(currentPlayer);
                        Debug.Log(currentPlayer + " has no cards left. Eliminated.");
                        if (playersWithNoCards.Count == 3)
                        {
                            string winner = "";
                            foreach (Player p in players)
                            {
                                if (p.HasCards)
                                {
                                    winner = p.name;
                                }
                            }
                            Debug.Log("Game finished, player: " + winner + " got all 52 cards");
                            yield break;
                        }
                    }
                }

                // Wait for a moment before the next player goes
                yield return new WaitForSeconds(playSpeed);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 1. DETECT INPUT: Check if the spacebar was pressed this frame.
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            HandleSlap();
        }
    }

    private bool CheckForSlapCondition()
    {
        if (pile.Count < 1) return false; // Can't slap an empty pile

        // Get the top card
        Card topCard = pile[pile.Count - 1];
        // Debug.Log($"Checking Card. Name: {topCard.Rank.ToString()}, Integer Value: {(int)topCard.Rank}");

        // Condition 1: Is the top card a Jack, Queen, or King?
        if (topCard.Rank == CardRank.Jack || topCard.Rank == CardRank.Queen || topCard.Rank == CardRank.King)
        {
            return true;
        }

        // Condition 2: Is it a pair? (Do the top two cards have the same rank?)
        if (pile.Count >= 2)
        {
            // Temporarily remove the top card to see the one underneath
            Card actualSecondCard = pile[pile.Count - 2];
            // Put the top card back

            if (topCard.Rank == actualSecondCard.Rank)
            {
                return true;
            }
        }

        return false; // No slap condition was met
    }


    private void HandleSlap()
    {
        if (slapSound != null)
        {
            audioSource.PlayOneShot(slapSound);
        }
        // // We'll assume the human is always the first player (players[0])
        Player humanPlayer = null;

        //We need to retrieve humanPlayer:
        foreach (Player p in players)
        {
            if (!p.IsBot)
            {
                humanPlayer = p;
            }
        }

        StartCoroutine(humanPlayer.AnimateSlap(centerPile, 0.2f, 1f));

        // 2. CHECK CONDITION: See if the slap was correct.
        if (CheckForSlapCondition())
        {
            pileClaimed = true;
            // 3A. CORRECT SLAP: Give the pile to the player.
            Debug.Log(humanPlayer.Name + " slapped correctly! You get the pile.");

            List<Card> wonCards = new List<Card>(centerPile.TakeAllCards());
            wonCards.Reverse(); // Reverse so the bottom of the pile is added first
            humanPlayer.AddCardsToHand(wonCards);
        }
        else
        {
            // 3B. INCORRECT SLAP: Player loses one health
            Debug.Log(humanPlayer.Name + " slapped incorrectly! You lose a card.");
            // if (humanPlayer.HasCards)
            // {
            //     Card penaltyCard = humanPlayer.DrawCard();
            //     centerPile.AddCardToBottom(penaltyCard); // Add their card to the bottom of the pile
            // }
        }
    }

    private void TriggerBotSlaps()
    {
        foreach (Player bot in players)
        {
            if (bot.IsBot)
            {
                StartCoroutine(BotSlapRoutine(bot));
            }
        }
    }

    private IEnumerator BotSlapRoutine(Player bot)
    {
        activeBotSlaps++;
        // Wait for a random time based on the bot's reaction speed
        float delay = Random.Range(bot.botReactionTime - 0.2f, bot.botReactionTime + 0.2f);
        yield return new WaitForSeconds(delay);

        // After waiting, check if the slap condition is still valid before slapping
        // This prevents the bot from slapping if a human has already won the pile
        if (CheckForSlapCondition())
        {
            yield return StartCoroutine(bot.AnimateSlap(centerPile, 0.2f, 1.5f));
            HandleBotSlap(bot);
        }
        activeBotSlaps--;
    }
    private void HandleBotSlap(Player bot)
    {
        if (pileClaimed) return;
        if (slapSound != null)
        {
            audioSource.PlayOneShot(slapSound);
        }
        pileClaimed = true;
        // The bot's slap is always correct because it only slaps when the condition is true.
        Debug.Log(bot.Name + " slapped correctly! They get the pile.");

        List<Card> wonCards = new List<Card>(centerPile.TakeAllCards());
        wonCards.Reverse(); // Reverse so the bottom of the pile is added first
        bot.AddCardsToHand(wonCards);
    }
}
