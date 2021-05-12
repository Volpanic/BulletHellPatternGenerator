using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    public CircleCollider2D BombCollider;
    public ContactFilter2D BulletLayer;

    public GameObject SpawnOnBullets;

    public float MaxRadius = 255;
    public float Duration = 4;

    private float timer = 0;
    private List<Collider2D> foundColliders = new List<Collider2D>();

    public ParticleSystem partSystem;
    private ParticleSystem.ShapeModule shape;

    // Start is called before the first frame update
    void Start()
    {
        BombCollider.radius = 0;
        if(partSystem != null)
        {
            shape = partSystem.shape;
            shape.donutRadius = BombCollider.radius;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        BombCollider.radius = Mathf.Lerp(0,MaxRadius,timer / Duration);
        BombCollider.OverlapCollider(BulletLayer, foundColliders);
        if(partSystem != null) shape.donutRadius = BombCollider.radius;

        for (int i = 0; i < foundColliders.Count; i++)
        {
            if(SpawnOnBullets != null) Instantiate(SpawnOnBullets, foundColliders[i].transform.position, Quaternion.identity);
            foundColliders[i].gameObject.SetActive(false);
            foundColliders.RemoveAt(i);
            i--;
        }

        timer += Time.fixedDeltaTime;
        if(timer >= Duration)
        {
            Destroy(gameObject);
        }
    }
}
