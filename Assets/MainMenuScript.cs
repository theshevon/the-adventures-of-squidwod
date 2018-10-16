using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {

    public Sprite img;
    public GameObject menu;

    public void StartGame()
    {
        GameObject.Find("Panel").GetComponent<Image>().sprite = img;

        GameObject[] objectsToDisable = GameObject.FindGameObjectsWithTag("Start");
        for (int i = 0; i < objectsToDisable.Length; i++){
            objectsToDisable[i].SetActive(false);
        }

        menu.SetActive(true);
    }

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
