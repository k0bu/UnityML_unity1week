using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSource : MonoBehaviour {

	public int addingScore = 1000;
	//public ParticleSystem particle;
	//public AudioSource pointSourceSE;
	//public GameObject particleObject;
	
	public void AddScore() {
		GameManager.instance.AnimateAddingScore(addingScore);
		GameManager.instance.PlayingPointSourceSE();
		//particleObject.SetActive(true);
		//particle.Play();
		//pointSourceSE.PlayOneShot(pointSourceSE.clip);
		gameObject.SetActive(false);
	}
}
