using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Obstacle : Agent {

	public void GameOver() {
		Done();
		SetReward(-1f);
	}
}
