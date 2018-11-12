using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PointSource : Agent {
	
	public void AddScore() {
		SetReward(1.0f);

		gameObject.SetActive(false);
	}
}
