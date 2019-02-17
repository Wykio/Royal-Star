using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerScript : MonoBehaviour
{
    const bool ALIVE = true;
    const bool DEAD = false;

    [SerializeField]
    private string playerName = "Test";

    [SerializeField]
    private bool state = ALIVE;

    [SerializeField]
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
            state = DEAD;
        }
    }

    void Update()
    {
        if (state == DEAD)
            return ;
    }
}
