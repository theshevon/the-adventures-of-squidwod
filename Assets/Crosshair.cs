using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {

	public Texture2D[] crosshairImage;
	public bool isActive;
	private int index;
	
	void OnGUI()
	{
		index = isActive ? index = 0 : index = 1;
		float xMin = Screen.width - (Screen.width - Input.mousePosition.x) - (crosshairImage[index].width / 16);
		float yMin = (Screen.height - Input.mousePosition.y) - (crosshairImage[index].height / 16);
		GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage[index].width/8, crosshairImage[index].height/8), crosshairImage[index]);
	}
}
