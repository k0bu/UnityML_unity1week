using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	[SerializeField] public GameObject StartModeUI;
	[SerializeField] public GameObject PlayModeUI;
	[SerializeField] public AudioSource pointSourceSE;
	
	[SerializeField] public Animator addingScoreAnimator;
	[SerializeField] public Text scoreText;
	[SerializeField] public Text addingScoreText;
	[SerializeField] public GameObject resultBG;
	[SerializeField] public Text resultScoreText;
	[SerializeField] public GameObject beforeGame;
	[SerializeField] public Text countDownText;
	[SerializeField] public Transform Player;
	public int score;
	public int addingScore;
	[SerializeField] public Text timerText;
	public float baseTime;
	private float playingTimer;
	private float beforeTimer = 5;

	public float scoreCounterDuration = .3f;
	float resultTimer;
	
	public enum GameState
	{
		START,
		BEFOREPLAY,
		PLAYING,
		SCOREVIEW
	}

	public static GameState currentState;
	

	void Awake() {
		if (instance == null)
			instance = this;
		else
			Destroy(this);

		playingTimer = baseTime;
		currentState = GameState.START;
		//timeElapsed = Time.time;

	}
	
	public void SwitchingStartToBefore() {
		currentState = GameState.BEFOREPLAY;
	}

	public void PlayingPointSourceSE() {
		pointSourceSE.PlayOneShot(pointSourceSE.clip);
	}
	
	void Update() {

		switch (currentState) {
			case GameState.START:
				Player.transform.position = new Vector3(.5f,.6f,.5f);
				Player.transform.rotation = Quaternion.identity;
				playingTimer = baseTime;
				beforeTimer = 5;
				resultTimer = 0;
				score = 0;
				scoreText.text = "Score: 000000";
				break;
			case GameState.BEFOREPLAY:
				Player.transform.position = new Vector3(.5f,.6f,.5f);
				Player.transform.rotation = Quaternion.identity;
				beforeGame.SetActive(true);
				beforeTimer -= Time.deltaTime;
				countDownText.text = beforeTimer.ToString("0");
				
				if (beforeTimer < 0) {
					
					beforeGame.SetActive(false);
					currentState = GameState.PLAYING;
				}
					
				break;
			case GameState.PLAYING:
				playingStateFunction();
				break;
			case GameState.SCOREVIEW:
				resultScoreText.text = "Result Score\n" + score.ToString();
				resultTimer += Time.deltaTime;
				if (Input.anyKey && resultTimer > 2f) {
					// naichilab.RankingLoader.Instance.SendScoreAndShowRanking (score);
					resultBG.SetActive(false);
					StartModeUI.SetActive(true);
					PlayModeUI.SetActive(false);
					currentState = GameState.START;
				}
					
				break;
		}
		
	}

	void playingStateFunction() {
		playingTimer -= Time.deltaTime;
		timerText.text = playingTimer.ToString("00");
		if (playingTimer < 0) {
			resultBG.SetActive(true);
			currentState = GameState.SCOREVIEW;
			
		}
	}

	public void AnimateAddingScore(int givenScore) {
		addingScoreText.text = "+ " + givenScore.ToString("0000");
		addingScoreAnimator.SetTrigger("Add");

		StartCoroutine(ScoreAnimation(score, score + givenScore, scoreCounterDuration));
	}
	
	public IEnumerator ScoreAnimation(float startScore, float endScore, float duration) {
		score = (int)endScore;
		print(score);
		float startTime = Time.time;
		float endTime = startTime + duration;
 
		while (Time.time < endTime) {
			float timeRate = (Time.time - startTime) / duration;
 
			float updateValue = (endScore - startScore) * timeRate + startScore;
			
			scoreText.text = "Score: " + updateValue.ToString("000000"); 
			yield return null;
			
		}
	
		scoreText.text = "Score: " + endScore.ToString("00000");
	}
}
