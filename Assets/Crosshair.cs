using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {

	public Texture2D crosshairImage;
	
	void OnGUI()
	{
		float xMin = Screen.width - (Screen.width - Input.mousePosition.x) - (crosshairImage.width / 16);
		float yMin = (Screen.height - Input.mousePosition.y) - (crosshairImage.height / 16);
		GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width/8, crosshairImage.height/8), crosshairImage);
	}
}
