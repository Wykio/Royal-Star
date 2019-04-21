using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerScript : MonoBehaviour
{
    [SerializeField]
    private BulletManagerScript bulletManager;

    [SerializeField]
    private Transform bulletPopPositionTransform;

    [SerializeField]
    private float popInterval;
    
    [SerializeField]
    private Rigidbody playerRigidbody;

    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private string playerName = "Test";

    [SerializeField]
    private bool activePlayer;


    private float nextPopTime = float.MinValue;
    private float speed = 10.0f;
    private bool alive = true;
    private int hp = 150;

    public string GetName()
    {
        return playerName;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0) {
            hp = 0;
            alive = false;
        }
    }

    void Update()
    {
        if (!alive || !activePlayer)
            return ;
        if (Input.GetMouseButtonDown(0) && Time.time > nextPopTime)
        {
            bulletManager.Shoot(bulletPopPositionTransform);
            nextPopTime = Time.time + popInterval;
        }
    }
}
