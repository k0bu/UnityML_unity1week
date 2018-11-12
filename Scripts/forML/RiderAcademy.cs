using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RiderAcademy : Academy {

	[SerializeField] Transform Player;	
	TileMapGenerator instance;

	void Awake()
	{
		instance = GetComponent<TileMapGenerator>();
	}

	//Initializing the game world
	private void SetEnvironment(){
		instance.StartGenerateMap();
		
		Player.transform.position = new Vector3(.5f,.6f,.5f);
		Player.transform.rotation = Quaternion.identity;

	}

	public override void InitializeAcademy(){
		


	}

	public override void AcademyReset(){
		instance.StartGenerateMap();
		
		Player.transform.position = new Vector3(.5f,.6f,.5f);
		Player.transform.rotation = Quaternion.identity;

	}

	public override void AcademyStep(){





	}


}
