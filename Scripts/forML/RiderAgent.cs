using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MLAgents;
using System;

public class RiderAgent : Agent {

    private RiderAcademy academy;
    public float timeBetweenDecisionsAtInference;
    private float timeSinceDecision;



    public float _walkSpeed = 1;

    Vector3 _moveAmount;
    Vector3 _smoothMoveVelocity;
    Rigidbody _rigidBody;
	bool inputPlus;
	bool inputMinus;
	private bool hasRotated;
	
	private float rotationSpeed = 90;
	private Quaternion rot;
	private Vector3 targetMoveAmount;


    private const int noAction = 0;
    private const int left = 1;
    private const int right = 2;

    public Queue<GameObject> pointCleared;

    public override void InitializeAgent(){
        _rigidBody = GetComponent<Rigidbody>();
        academy = FindObjectOfType(typeof(RiderAcademy)) as RiderAcademy;
        pointCleared = new Queue<GameObject>();
    }

    public override void CollectObservations(){
        AddVectorObs(gameObject.transform);
        AddVectorObs(gameObject.transform.forward);

        // var positionX = (int) transform.position.x;
        // var positionZ = (int) transform.position.z;
        // var maxPosition = academy.gridSize - 1;
    }


    public override void AgentAction(float[] vectorAction, string textAction){
        AddReward(.01f);
        int action = Mathf.FloorToInt(vectorAction[0]);
        switch (action)
        {
            case noAction:
                // AddReward(-.01f);
                // do nothing
                break;
            case left:
                AddReward(.02f);
                inputMinus = true;
                break;
            case right:
                AddReward(.02f);
                inputPlus = true;
                break;
            default:
                throw new ArgumentException("Invalid action value");
        }


    }

    private void Update() {
		
		hasRotated = false;
		
        // Calculate movement:
        // inputPlus = Input.GetButtonDown("Right");
        // inputMinus = Input.GetButtonDown("Left");

		PerpendicularRotator();
	
		//only move forward
        var moveDir = transform.forward.normalized;
		
        targetMoveAmount = moveDir * _walkSpeed;
        //_moveAmount = Vector3.SmoothDamp(_moveAmount,targetMoveAmount,ref _smoothMoveVelocity,.15f);
		//print(targetMoveAmount);

		if (hasRotated) {
			transform.position = new Vector3(Mathf.FloorToInt(transform.position.x) + .5f, transform.position.y, Mathf.FloorToInt(transform.position.z) + .5f);
		}
    }
    
    private void FixedUpdate() {
        WaitTimeInference();

        Vector3 localMove = targetMoveAmount* Time.fixedDeltaTime;
        	
        //Vector3 localMove = transform.TransformDirection(targetMoveAmount) * Time.fixedDeltaTime;
		_rigidBody.MovePosition(_rigidBody.position + localMove);
        
		
    }
    
    private void PerpendicularRotator() {
		rotationSpeed = 0;
		rot = Quaternion.identity;

		if (inputPlus) {
			rotationSpeed += 90;
			hasRotated = true;
            inputPlus = false;
		}


		if (inputMinus) {
			rotationSpeed -= 90;
			hasRotated = true;
            inputMinus = false;
		}
			

		rot = Quaternion.AngleAxis(rotationSpeed, transform.up);
		
		Quaternion destRot = rot * transform.localRotation;

		transform.rotation = Quaternion.Slerp(rot,destRot,2 );

	}

    private void OnTriggerEnter(Collider other) {
        //Debug.Log("CollisionEnter");
        Collider[] blockTest = Physics.OverlapBox(transform.position+transform.forward*.4f, new Vector3(0.3f, 0.3f, 0.3f));

		if (other.GetComponent<PointSource>()){
            Debug.Log("point");
            SetReward(10f);
            pointCleared.Enqueue(other.gameObject);
            other.GetComponent<PointSource>().AddScore();
        }
			

		if(other.GetComponent<Obstacle>()){
            if (blockTest.Where(col => col.gameObject.CompareTag("pit")).ToArray().Length == 1){
                Debug.Log("pit");
                Done();
                SetReward(-2f);
            }
        }
			
		
	}
    


    
    
    // to be implemented by the developer
    public override void AgentReset(){
        for(int i = 0; i < pointCleared.Count; i++){
            pointCleared.Dequeue().SetActive(true);
        }
        academy.AcademyReset();
    }

    private void WaitTimeInference(){
        if (!academy.GetIsInference()){
            RequestDecision();
        }
        else{
            if (timeSinceDecision >= timeBetweenDecisionsAtInference){
                timeSinceDecision = 0f;
                RequestDecision();
            }
            else{
                timeSinceDecision += Time.fixedDeltaTime;
            }
        }
    }




	
}
