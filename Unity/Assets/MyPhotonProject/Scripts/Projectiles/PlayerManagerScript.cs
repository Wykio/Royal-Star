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

    [SerializeField]
    private bool activePlayer;

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
        if (Input.GetKey(KeyCode.D))
            playerTransform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * speed);
        if (Input.GetKey(KeyCode.A))
            playerTransform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * speed);
    }
}
