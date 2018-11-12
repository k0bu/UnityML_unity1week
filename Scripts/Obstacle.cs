using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

	public void GameOver() {
		GameManager.instance.resultBG.SetActive(true);
		GameManager.currentState = GameManager.GameState.SCOREVIEW;
	}
}
