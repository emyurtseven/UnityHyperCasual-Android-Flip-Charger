using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    float moveSpeed;

    float despawnPosX;

    Rigidbody2D myRigidbody;
    BoxCollider2D boxCollider;
    float playerPosX;

    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    void Awake()
    {
        playerPosX = GameObject.FindGameObjectWithTag("Player").transform.position.x;
        despawnPosX = ScreenUtils.ScreenLeft - 2f;
    }

    private void Start() 
    {
        EventManager.AddNoArgumentListener(OnDifficultyUpListener, EventType.DifficultyUp);
    }

    private void OnEnable() 
    {
        boxCollider = GetComponent<BoxCollider2D>();
        myRigidbody = GetComponent<Rigidbody2D>();
        
        myRigidbody.velocity = new Vector2(-moveSpeed, 0);

        StartCoroutine(CheckOutOfScreen());
    }

    /// <summary>
    /// Listener for difficulty up event which is invoked from GameManager
    /// </summary>
    private void OnDifficultyUpListener()
    {
        moveSpeed = GameManager.Instance.GameSpeed;
        myRigidbody.velocity = new Vector2(-moveSpeed, 0);
    }

    private IEnumerator CheckOutOfScreen()
    {
        WaitForSeconds delay = new WaitForSeconds(0.1f);
        while (boxCollider.bounds.max.x > playerPosX)
        {
            yield return delay;
        }

        GameManager.Instance.ScoreUp();

        while (transform.position.x > despawnPosX)
        {
            yield return delay;
        }

        if(!Enum.TryParse("Obstacles_" + gameObject.name, out PooledObjectType obstacleType))
        {
            Debug.LogWarning($"Enum {gameObject.name} doesn't exist as a PooledObjectType. " + 
                                "Check pooled object enums and make sure object name matches it.");
        }

        ObjectPool.ReturnPooledObject(obstacleType, gameObject);
    }
}
