using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public Collider2D Collider;
    public ContactFilter2D CorrectLayer;
    public SpriteRenderer sRenderer;

    private Color col = Color.white;
    private Collider2D[] cols = new Collider2D[1];

    void FixedUpdate()
    {
        if(sRenderer != null)
        {
            col.a = Mathf.Lerp(0f,1f,Time.deltaTime * 4f);
            sRenderer.color = col;
        }

        if(Collider!= null && Collider.OverlapCollider(CorrectLayer, cols) > 0)
        {
            if(cols[0] != null) cols[0].gameObject.BroadcastMessage("Hurt", 1,SendMessageOptions.DontRequireReceiver);
            gameObject.SetActive(false);
        }
    }
}
