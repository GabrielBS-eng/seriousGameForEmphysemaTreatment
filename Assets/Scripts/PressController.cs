﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PressController : MonoBehaviour
{
    GameManager game;

    public Animator animator;

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;

    //for the movement forward
    public float runSpeed;
    //public float forwardMove = 0;

    public float pressForce;
    public Vector3 startPos;

    new Rigidbody2D rigidbody;

    public float vel;

    public static bool gameOver;

    public static float countDown;
    private float countDown_aux;

    public int countPauses;
    private bool startCountOnce;
    public float totalExerciseTime;
    public Text totalExerciseTimeResult;
    public Text totalPausesResult;

    public enum gameState
    {
        scene,
        beginning,
        newCicle,
        duringCicle,
        theEnd,
        none
    };

    public static gameState state;

    void Start()
    {
        game = GameManager.Instance;
    }

    void OnEnable()
    {
        countPauses = 0;
        gameOver = false;

        countDown = float.Parse(Parameters.countDown);
        countDown_aux = countDown;

        runSpeed = 6.75f;
        animator.SetFloat("forwardMove", 0.2f);
        pressForce = 4f;
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.simulated = true;
        rigidbody.gravityScale = 0;
        rigidbody.angularDrag = 0;
        state = gameState.scene;

        startCountOnce = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (game.GameOver) return;
        if (state == gameState.duringCicle) Fly();
    }

    //for the movement forward
    void FixedUpdate()
    {
        switch (state)
        {
            case gameState.scene:
                transform.Translate(new Vector2(animator.GetFloat("forwardMove"), 0));
                break;

            case gameState.beginning:
                ForwardMove(new Vector2(animator.GetFloat("forwardMove"), rigidbody.angularDrag*Time.fixedDeltaTime + rigidbody.velocity.y));
                if (animator.GetBool("grounded") == true) state = gameState.duringCicle;
                break;

            case gameState.duringCicle:
                rigidbody.gravityScale = 0;
                ForwardMove(new Vector2(animator.GetFloat("forwardMove"), vel));
                //if(Input.GetButtonDown("Jump")) transform.Translate(new Vector2(animator.GetFloat("forwardMove"), pressForce * Time.fixedDeltaTime));
                //if(Input.GetButtonDown("Jump")) rigidbody.MovePosition(rigidbody.position + new Vector2(0, pressForce * Time.fixedDeltaTime));
                break;

            case gameState.newCicle:
                rigidbody.gravityScale = 1;
                ForwardMove(new Vector2(animator.GetFloat("forwardMove"), rigidbody.angularDrag*Time.fixedDeltaTime + rigidbody.velocity.y));
                if (animator.GetBool("grounded") == true && !Input.GetButton("Jump")) state = gameState.duringCicle;
                break;

            case gameState.theEnd:
                totalExerciseTimeResult.text = totalExerciseTime.ToString("F2") + " segundos";
                totalPausesResult.text = countPauses.ToString();
                //transform.position = new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z);
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "DeadZone")
        {
            rigidbody.simulated = false;
            animator.SetBool("grounded", true);
            animator.SetFloat("forwardMove", 0f);
        }
    }

    void ForwardMove(Vector2 direction)
    {
        rigidbody.velocity = direction;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        switch(col.gameObject.tag)
        {
            case ("TriggerZone"):
                ForwardMove(new Vector2(animator.GetFloat("forwardMove"), 4f));
                state = gameState.newCicle;
                rigidbody.simulated = true;
                animator.SetFloat("forwardMove", runSpeed);
                rigidbody.gravityScale = 1;
                rigidbody.angularDrag = 0.05f;
                break;

            case ("BeginningZone"):
                state = gameState.beginning;

                rigidbody.simulated = true;
                animator.SetFloat("forwardMove", 10f);
                rigidbody.gravityScale = 0.25f;
                rigidbody.angularDrag = 0.05f;
                break;

            case ("EndGame"):
                Debug.Log("cab");
                state = gameState.theEnd;
                totalExerciseTime = Time.realtimeSinceStartup - totalExerciseTime;
                break;

            default:
                animator.SetFloat("forwardMove", 0);
                break;
        }
    }

    void Fly()
    {
        if(Input.GetButton("Jump"))
        {
            if(startCountOnce)
            {
                totalExerciseTime = Time.realtimeSinceStartup;
                startCountOnce = false;
            }
            rigidbody.simulated = true;
            animator.SetBool("grounded", false);
            vel = 11.605f/countDown;
        }
        else
        {
            vel = 0;
        }
        if (Input.GetButtonUp("Jump")) countPauses++;
    }
}
