using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour {

    static int difficulty = 0; // 0 for easy
                               // 1 for hard

    public void SetDifficulty(int value){
        difficulty = value;
        Debug.Log(difficulty == 0 ? "Easy" : "Hard");
    }

    public int GetDifficulty(){
        return difficulty;
    }
}
