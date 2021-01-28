using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManagerSingleton : MonoBehaviour
{
    [SerializeField] private int _playerLives = 3;
    private int _playerScore;

    [SerializeField] TMP_Text livesText;
    [SerializeField] TMP_Text scoreText;





    private void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        _playerScore = 0;

        livesText.text = _playerLives.ToString();
        scoreText.text = _playerScore.ToString();
    }

 



    /*********************************** Methods ***************************************************/
    public void addToScore(int pointsToAdd)
    {
        _playerScore += pointsToAdd;
        scoreText.text = _playerScore.ToString();
    }
    
    public void processPlayerDeath()
    {
        if (_playerLives > 1)
            takeLife();
        else
            resetGameSession();
    }

    private void takeLife()
    {
        --_playerLives;
        livesText.text = _playerLives.ToString();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    private void resetGameSession()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }






    /*********************************** Getters and Setters ***************************************************/
    public int GetPlayerScore() { return _playerScore; }
    public void SetPlayerScore(int value) { _playerScore = value; }
}
