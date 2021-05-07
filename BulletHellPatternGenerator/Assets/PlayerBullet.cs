using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public Collider2D Collider;
    public ContactFilter2D CorrectLayer;

    private Collider2D[] cols = new Collider2D[1];

    void FixedUpdate()
    {
        if(Collider!= null && Collider.OverlapCollider(CorrectLayer, cols) > 0)
        {
            if(cols[0] != null) cols[0].gameObject.BroadcastMessage("Hurt", 1,SendMessageOptions.DontRequireReceiver);
            gameObject.SetActive(false);
        }
    }
}
