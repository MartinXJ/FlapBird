using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class TapController : MonoBehaviour
{   
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltsmooth = 5; // to rotate downwards, as the bird falls
    public Vector3 startPos;

    Rigidbody2D rbody;
    Quaternion downRotation; // fancy form of rotation, vector 4 xyzw
    Quaternion forwardRotation;

    GameManager instGame;

    
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90); // x = 0, y = 0, z = -90
        forwardRotation = Quaternion.Euler(0, 0, 45);
        instGame = GameManager.GameInstance;

        // initially, after we start the game. The bird does not get affected by physics
        // rbody.simulated = false; 
    }

    // Update is called once per frame
    void Update()
    {
        if (instGame.GameOver) return;
        // 0 is left click, 1 is right click
        if (Input.GetMouseButtonDown(0)) {
            // transform rotation
            rbody.velocity = Vector3.zero;
            transform.rotation = forwardRotation;
            rbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force); // Try ForceMode2D.Impulse
        }
        else {
            // Lerp from a source value to a target value over certain amount of time
            transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltsmooth * Time.deltaTime);
        }

    }

    void OnTriggerEnter2D(Collider2D col){
        // Whether we hit a score zone or a dead zone
        if (col.gameObject.tag == "ScoreZone"){
            // register a score
            OnPlayerScored(); // event will be sent to GameManager

            // play sound
        }

        else if (col.gameObject.tag == "DeadZone"){
            // when it hits, the game stops. The bird is ded
            rbody.simulated = false;
            // register a dead event
            OnPlayerDied(); // event will be sent to GameManager

            // play a sound


        }
    }

    void OnEnable() {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable() {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted() {
        // reset the velocity
        rbody.velocity = Vector3.zero;
        rbody.simulated = true;

    }

    void OnGameOverConfirmed() {
        transform.localPosition  = startPos;
        transform.rotation = Quaternion.identity;
    }


}
