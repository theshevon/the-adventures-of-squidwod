using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartButtonScript : MonoBehaviour {

    const float blinkFreq = 0.5f;
    float countdown = blinkFreq;
    public TextMeshProUGUI text;

    void Update() {
        countdown -= Time.deltaTime;
        if (countdown <= 0){
            countdown = blinkFreq;
            text.enabled = !text.enabled;
        }
	}
}
