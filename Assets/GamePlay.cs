using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    private GameManager gameManager;
    private List<Player> players;
    private CenterPile centerPile;
    private Stack<Card> pile;

    // reference the script
    void Awake()
    {
        gameManager = GetComponent<GameManager>();
        centerPile = GetComponent<CenterPile>();
        Debug.Log(centerPile);
        players = gameManager.players;
        pile = centerPile.pile;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(players.Count);
        Debug.Log(pile.Count);
    }

    private void InitGame() {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
