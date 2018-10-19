using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialSlidesScript : MonoBehaviour {

    public Sprite[] images;
    int index;

	void Update () {

        if (Input.anyKeyDown){
            index += 1;
            if (index == images.Length)
            {
                gameObject.SetActive(false);
                index = 0;
                //SceneManager.LoadScene("Menu");
            }
            if (index < images.Length) gameObject.GetComponent<Image>().sprite = images[index];
            
        }

	}
}
