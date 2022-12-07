using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameScript : MonoBehaviour {

    public List<GameObject> TotalCountGO;
    public int TotalCollected;
    public float TotalTime;
    public string FormattedTime;

    public TextMeshProUGUI CurrentTime;
    public TextMeshProUGUI TotalScoreText;
    public TextMeshProUGUI CurrentScore;
    public TextMeshProUGUI SpeedBoost;
    public TextMeshProUGUI JumpPower;
    public TextMeshProUGUI LowGravity;
    public TextMeshProUGUI GiantMarble;

    public Ball ball;
    public BallUserControl ballControl;

    private bool _isRunning;
    private bool _wasRunningLastUpdate;
    private float _elapsedSeconds;
    private float _timeLastUpdate;


    // Use this for initialization
    void Start () {
        TotalCountGO = new List<GameObject>(GameObject.FindGameObjectsWithTag("Collectable"));
        TotalScoreText.text = TotalCountGO.Count.ToString();
        StartTimer();
        SpeedBoost.text = " ";
        LowGravity.text = " ";
        JumpPower.text = " ";
        GiantMarble.text = " ";

    }
	
	// Update is called once per frame
	void Update () {
        
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
    private void LateUpdate() {
        StartCoroutine(updateTextMesh());
       
    }
    public void StartTimer() {
        _isRunning = true;
    }

    public void ResetTimer() {
        _elapsedSeconds = 0;
    }

    public void StopTimer() {
        _isRunning = false;
    }
    public void UpdateScore() {
        CurrentScore.text = TotalCollected.ToString();
    }
    private IEnumerator updateTextMesh() {
        if (!_isRunning) {
            _wasRunningLastUpdate = false;
            //return;
        }

        if (_wasRunningLastUpdate) {
            var deltaTime = Time.time - _timeLastUpdate;
            _elapsedSeconds += deltaTime;
        }

        var timeSpan = TimeSpan.FromSeconds(_elapsedSeconds);
        CurrentTime.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        _timeLastUpdate = Time.time;
        _wasRunningLastUpdate = true;
        yield return CurrentTime;
    }
}
