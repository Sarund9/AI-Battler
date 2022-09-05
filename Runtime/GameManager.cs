using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager I;

    [SerializeField]
    Text display;

    [SerializeField]
    List<GameObject> disableOnPlay;

    static string Display
    {
        get => I.display.text;
        set => I.display.text = value;
    }

    //bool redWins = false;
    //bool blueWins = false;
    bool wins = false;

    public static event Action<Team?> OnGameOver;
    public static event Action OnGameStart;

    private void Awake()
    {
        I = this;
    }

    public void StartGame()
    {
        foreach (var item in disableOnPlay)
        {
            item.SetActive(false);
        }
        
        OnGameStart?.Invoke();
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public static void Yield(Team team)
    {
        if (I.wins)
            return;
        Team winner;
        if (team == Team.Blue)
            winner = Team.Red;
        else
            winner = Team.Blue;

        I.wins = true;
        Display = $"{winner} team wins";
        OnGameOver?.Invoke(winner);

    }

}