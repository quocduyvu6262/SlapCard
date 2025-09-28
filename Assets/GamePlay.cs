using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("Lives System")]
    public int maxLives = 3;
    private int currentLives;
    public Transform livesPanel;       // The container for hearts
    public GameObject heartPrefab;
    private List<GameObject> heartIcons = new List<GameObject>();  // The heart template (disabled Image)

    [Header("Timer System")]
    public float gameDuration = 60f; // total game time in seconds
    private float remainingTime;
    public TextMeshProUGUI timerText;


    private bool isSlapping = false;

    [Header("UI Feedback")]
    public GameObject floatingTextPrefab; // The prefab you just created
    public Transform worldCanvas;         // The World Space Canvas


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
        currentLives = maxLives;
        if (heartPrefab != null)
        {
            heartPrefab.SetActive(false); // Hide template
        }
        CreateHearts();

        // Start timer
        remainingTime = gameDuration;
        UpdateTimerUI();
        StartCoroutine(TimerRoutine());
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
            StartCoroutine(HandleSlap());
        }
    }

    private IEnumerator HandleSlap()
    {
        if (isSlapping) yield break; // Ignore if already slapping
        isSlapping = true;

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

        yield return StartCoroutine(humanPlayer.AnimateSlap(centerPile, 0.2f, 1f));

        // 2. CHECK CONDITION: See if the slap was correct.
        if (CheckForSlapCondition() && TryClaimPile())
        {
            Card correctCard = centerPile.PeekTopCard();
            // 3A. CORRECT SLAP: Give the pile to the player.
            Debug.Log(humanPlayer.Name + " slapped correctly on " + correctCard.name + "! You get the pile.");

            List<Card> wonCards = new List<Card>(centerPile.TakeAllCards());
            wonCards.Reverse(); // Reverse so the bottom of the pile is added first
            humanPlayer.AddCardsToHand(wonCards);
            ShowFloatingText($"+{wonCards.Count}", humanPlayer.transform.position, humanPlayer.transform.rotation);
        }
        else
        {
            // 3B. INCORRECT SLAP: Player loses one health
            if (centerPile.pile.Count > 0)
                LoseLife();
            Debug.Log(humanPlayer.Name + " slapped incorrectly! You lose a life.");
            // if (humanPlayer.HasCards)
            // {
            //     Card penaltyCard = humanPlayer.DrawCard();
            //     centerPile.AddCardToBottom(penaltyCard); // Add their card to the bottom of the pile
            // }
        }

        yield return new WaitForSeconds(0.2f);
        isSlapping = false;
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

        yield return StartCoroutine(bot.AnimateSlap(centerPile, 0.2f, 1.5f));

        // After waiting, check if the slap condition is still valid before slapping
        // This prevents the bot from slapping if a human has already won the pile
        if (CheckForSlapCondition() && TryClaimPile())
        {
            HandleBotSlap(bot);
        }
        activeBotSlaps--;
    }
    private void HandleBotSlap(Player bot)
    {
        if (slapSound != null)
        {
            audioSource.PlayOneShot(slapSound);
        }
        Card correctCard = centerPile.PeekTopCard();

        // The bot's slap is always correct because it only slaps when the condition is true.
        Debug.Log(bot.Name + " slapped correctly on " + correctCard.name + "! They get the pile.");

        List<Card> wonCards = new List<Card>(centerPile.TakeAllCards());
        wonCards.Reverse(); // Reverse so the bottom of the pile is added first
        bot.AddCardsToHand(wonCards);
        ShowFloatingText($"+{wonCards.Count}", bot.transform.position,  bot.transform.rotation);
    }

    private IEnumerator TimerRoutine()
    {
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerUI();
            yield return null;
        }

        remainingTime = 0;
        UpdateTimerUI();

        // Time's up! Decide the winner
        DetermineWinnerByCards();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    // Helpers
    private void CreateHearts()
    {
        // Clear old
        foreach (Transform child in livesPanel)
        {
            if (child.gameObject != heartPrefab)
                Destroy(child.gameObject);
        }

        heartIcons.Clear();

        // Create hearts
        for (int i = 0; i < maxLives; i++)
        {
            GameObject heart = Instantiate(heartPrefab, livesPanel);
            heart.SetActive(true);
            heartIcons.Add(heart);
        }
    }

    private void LoseLife()
    {
        if (currentLives <= 0) return;

        currentLives--;
        Destroy(heartIcons[currentLives]); // Remove last heart
        heartIcons.RemoveAt(currentLives);

        if (currentLives <= 0)
        {
            Debug.Log("You lost all lives! Game Over.");
            EndGame(false);
        }
    }

    private void EndGame(bool playerWon)
    {
        if (playerWon)
        {
            Debug.Log("🎉 You win!");
            StopAllCoroutines();
            SceneManager.LoadScene(3);
        }
        else
        {
            Debug.Log("💀 You lost!");
            StopAllCoroutines();
            SceneManager.LoadScene(4);
        }

        StopAllCoroutines();
    }

    private bool TryClaimPile()
    {
        if (pileClaimed) return false;
        pileClaimed = true;
        return true;
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

    private void DetermineWinnerByCards()
    {
        Player winner = null;
        int maxCards = -1;

        foreach (Player p in players)
        {
            if (p.CardCount > maxCards)
            {
                maxCards = p.CardCount;
                winner = p;
            }
        }

        if (winner != null)
        {
            bool playerWon = !winner.IsBot;
            EndGame(playerWon);
        }
        else
        {
            EndGame(false);
        }
    }

    /// <summary>
    /// Creates a floating text message at a specific world position.
    /// </summary>
    private void ShowFloatingText(string message, Vector3 position, Quaternion rotation)
    {
        if (floatingTextPrefab == null) return;

        // Instantiate the text prefab as a child of the world canvas

        float zAngle = rotation.eulerAngles.z;
        float offset = 2.5f;

        if (Mathf.Approximately(zAngle, 90f)) 
        {
            position.x -= offset;
        } 
        else if (Mathf.Approximately(zAngle, 180f)) 
        {
            position.y -= offset;
        }
        else if (Mathf.Approximately(zAngle, 270f)) 
        {
            position.x += offset;
        } 
        else 
        {
            position.y += offset;
        }

        position.z -= 4f;

        GameObject textObj = Instantiate(floatingTextPrefab, position, Quaternion.identity, worldCanvas);

        // Set the text content
        TextMeshProUGUI textMesh = textObj.GetComponent<TextMeshProUGUI>();
        if (textMesh != null)
        {
            textMesh.text = message;
        }
    }
}
