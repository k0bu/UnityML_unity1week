using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RiderAcademy : Academy {

	[SerializeField] Transform Player;	
	TileMapGenerator instance;

	public int gridSize;

	// void Awake()
	// {
	// 	instance = GetComponent<TileMapGenerator>();
	// }

	//Initializing the game world
	private void SetEnvironment(){

		
		
		Player.transform.position = new Vector3(.5f,.6f,.5f);
		Player.transform.rotation = Quaternion.identity;

	}

	public override void InitializeAcademy(){
		gridSize = (int)resetParameters["gridSize"];
		instance = GetComponent<TileMapGenerator>();
		
		instance.StartGenerateMap((int)resetParameters["gridSize"],
			(float)resetParameters["percentObstacles"] ,
			(int)resetParameters["numberPoint"] );

		SetEnvironment();


	}

	public override void AcademyReset(){
		SetEnvironment();

	}

	public override void AcademyStep(){





	}


}
