using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSlidesScript : MonoBehaviour {

    public Sprite[] images;
    int index = 0;
	

	void Update () {
        if (Input.anyKeyDown){
            index += 1;
            if (index == images.Length){
                index = 0;
            }
            gameObject.GetComponent<Image>().sprite = images[index];
        }
	}
}
