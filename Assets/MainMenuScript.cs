using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {

    public Sprite img;
    public Sprite img2;
    public GameObject mainMenu;
    public GameObject difficultyMenu;

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void StartGame()
    {
        // change from title screen to menu screen
        GameObject.Find("Panel").GetComponent<Image>().sprite = img;

        // disable start button
        GameObject[] objectsToDisable = GameObject.FindGameObjectsWithTag("Start");
        for (int i = 0; i < objectsToDisable.Length; i++){
            objectsToDisable[i].SetActive(false);
        }

        // display menu options
        mainMenu.SetActive(true);
    }

    public void OpenDifficultyMenu(){
        mainMenu.SetActive(false);
        difficultyMenu.SetActive(true);
    }

    /// <summary>
    /// Goes back to main menu from difficulty menu.
    /// </summary>
    public void BackToMain(){
        difficultyMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    /// <summary>
    /// Sets the difficulty to easy.
    /// </summary>
    public void SetEasy()
    {
        gameObject.GetComponent<GameSettings>().SetDifficulty(0);
    }

    /// <summary>
    /// Sets the difficulty to hard.
    /// </summary>
    public void SetHard()
    {
        gameObject.GetComponent<GameSettings>().SetDifficulty(1);
    }

    public void SetSexy()
    {
        GameObject.Find("Panel").GetComponent<Image>().sprite = img2;
    }

    /// <summary>
    /// Loads the game scene.
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
