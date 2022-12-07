using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {

    public GameObject ballPlayer;
    public PlayerBall ballScript;
    public PlayerBallUserController ballControlScript;
    public GameScript _gs;
	// Use this for initialization
	void Start () {
        ballScript = ballPlayer.GetComponent<PlayerBall>();
        ballControlScript = ballPlayer.GetComponent<PlayerBallUserController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn() {
        this.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<Collider>().enabled = false;
        if(this.name == "SpeedPowerUp") {
            ballControlScript.hasSpeed = true;
            _gs.SpeedBoost.text = "Speed Boost";
        }
        if(this.name == "GravityPowerUp") {
            ballScript.hasGravity = true;
            _gs.LowGravity.text = "Low Gravity";
        }
        if(this.name == "GiantPowerUp") {
            ballScript.hasGiant = true;
            _gs.GiantMarble.text = "Gaint Power";
        }
        if(this.name == "JumpPowerUp") {
            ballScript.hasJump = true;
            _gs.JumpPower.text = "Super Jump";
        }
        yield return new WaitForSeconds(15f);
        this.GetComponent<MeshRenderer>().enabled = true;
        this.GetComponent<Collider>().enabled = true;
    }
}
