using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

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

	private void Awake() {
        _rigidBody = GetComponent<Rigidbody>();
    }

	private void Update() {
		
		if (GameManager.currentState != GameManager.GameState.PLAYING) return;
		
		hasRotated = false;
		
        // Calculate movement:
        inputPlus = Input.GetButtonDown("Right");
        inputMinus = Input.GetButtonDown("Left");

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
		if (GameManager.currentState != GameManager.GameState.PLAYING) return;
        // Apply movement to rigidbody
		
        //Vector3 localMove = transform.TransformDirection(targetMoveAmount) * Time.fixedDeltaTime;
		Vector3 localMove = targetMoveAmount* Time.fixedDeltaTime;
        _rigidBody.MovePosition(_rigidBody.position + localMove);
		
    }

	private void PerpendicularRotator() {
		rotationSpeed = 0;
		rot = Quaternion.identity;

		if (inputPlus) {
			rotationSpeed += 90;
			hasRotated = true;
		}


		if (inputMinus) {
			rotationSpeed -= 90;
			hasRotated = true;
		}
			

		rot = Quaternion.AngleAxis(rotationSpeed, transform.up);
		
		Quaternion destRot = rot * transform.localRotation;

		transform.rotation = Quaternion.Slerp(rot,destRot,1 );

	}

	private void OnCollisionEnter(Collision other) {
		if (GameManager.currentState != GameManager.GameState.PLAYING) return;
		
		if (other.collider.GetComponent<PointSource>())
			other.collider.GetComponent<PointSource>().AddScore();
		if(other.collider.GetComponent<Obstacle>())
			other.collider.GetComponent<Obstacle>().GameOver();
		
	}
	
}
