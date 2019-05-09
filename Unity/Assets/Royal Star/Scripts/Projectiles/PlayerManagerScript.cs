using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerScript : MonoBehaviour
{
    
    [SerializeField]
    private Rigidbody playerRigidbody;

    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private string playerName = "Test";

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
        if (!alive)
            return ;
        /*if (Input.GetMouseButtonDown(0) && Time.time > nextPopTime)
        {
            bulletManager.Shoot(bulletPopPositionTransform);
            nextPopTime = Time.time + popInterval;
        }*/
    }
}
