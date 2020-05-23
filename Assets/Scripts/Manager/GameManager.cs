using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Difficulty currentDifficulty;
    public enum Difficulty
    {
        easy, medium, hard
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChooseDifficulty(int index)
    {
        LoadDifficulty((Difficulty)index);
    }

    private void LoadDifficulty(Difficulty chosenDifficulty)
    {
        SceneManager.LoadScene(chosenDifficulty.ToString() + "Difficulty");
    }

    public void ResetWordSearch()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
