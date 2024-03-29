﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public const int MAX_LIFE = 10;
    public const int MAX_BOMB = 10;

    public float MoveSpeed = 10f;
    public float ShiftMoveSpeed = 2.5f;

    public int Lives = 3;
    public int Bombs = 3;

    public int Score = 0;
    public int Graze = 0;

    public SpriteRenderer sRenderer;

    [Header("Colliders")]
    public Collider2D BaseCollider;
    public Collider2D GrazeCollider;
    public ContactFilter2D BulletMask;
    public Collider Space;

    [Header("Prefabs")]
    public GameObject WorldBomb;

    [Header("Effects")]
    public ParticleSystem GrazeEffect;

    [Header("UI")]
    public HorizontalLayoutGroup LivesCounter;
    public HorizontalLayoutGroup BombsCounter;
    public GameObject Life;
    public GameObject Bomb;
    private GameObject[] inWorldLives;
    private GameObject[] inWorldBombs;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI GrazeText;

    public UnityEvent OnDeath;

    private float hitTimer = 0;
    private bool rHit = false;

    // Start is called before the first frame update
    void Start()
    {
        //Create lives in layout group
        if (LivesCounter != null && Life != null)
        {
            inWorldLives = new GameObject[MAX_LIFE];
            for (int i = 0; i < inWorldLives.Length; i++)
            {
                inWorldLives[i] = Instantiate(Life, LivesCounter.transform);
                if (i > Lives) inWorldLives[i].SetActive(false);
            }
        }

        //Create bomb counter
        if (BombsCounter != null && Bomb != null)
        {
            inWorldBombs = new GameObject[MAX_BOMB];
            for (int i = 0; i < inWorldBombs.Length; i++)
            {
                inWorldBombs[i] = Instantiate(Bomb, BombsCounter.transform);
                if (i > Bombs-1) inWorldBombs[i].SetActive(false);

                rHit = true;
                hitTimer = 3f;
            }
        }
    }

    private Color clearColor = new Color(0.1f,0.1f,0.1f,1f);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && Bombs > 0)
        {
            Bombs--;
            if (WorldBomb != null) Instantiate(WorldBomb, transform.position, Quaternion.identity);
            UpdateBombs();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"), 0).normalized;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            sRenderer.color = clearColor;
            transform.position += direction * ShiftMoveSpeed * Time.fixedDeltaTime;
        }
        else
        {
            sRenderer.color = Color.white;
            transform.position += direction * MoveSpeed * Time.fixedDeltaTime;
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, Space.bounds.min.x, Space.bounds.max.x),
            Mathf.Clamp(transform.position.y, Space.bounds.min.y, Space.bounds.max.y), transform.position.z);

        if (rHit)
        {
            hitTimer -= Time.fixedDeltaTime;
            if (hitTimer <= 0)
            {
                rHit = false;
                sRenderer.enabled = true;
            }
            else
            {
                if (hitTimer % 0.1f < 0.05f)
                {
                    sRenderer.enabled = false;
                }
                else
                {
                    sRenderer.enabled = true;
                }
            }
        }

        //Collisions
        Collider2D[] result = new Collider2D[1];

        if (BaseCollider != null)
        {
            //Regular Hurt
            if (!rHit && BaseCollider.OverlapCollider(BulletMask, result) > 0)
            {
                Lives--;
                UpdateLives();
                rHit = true;
                hitTimer = 3f;
            }
        }

        if (GrazeCollider != null)
        {
            //Regular Hurt
            int grazeAmount = GrazeCollider.OverlapCollider(BulletMask, result);

            if(grazeAmount > 0)
            {
                Graze += grazeAmount;
                if (GrazeText != null) GrazeText.text = Graze.ToString();

                if (GrazeEffect != null)
                {
                    GrazeEffect.Stop();
                    GrazeEffect.Play();
                }
            }
        }
    }

    public void AddPoints(int PointAmount)
    {
        Score += PointAmount;
        if (ScoreText != null) ScoreText.text = Score.ToString();
    }

    void UpdateLives()
    {
        //Create lives in layout group
        if (LivesCounter != null && Life != null)
        {
            for (int i = 0; i < inWorldLives.Length; i++)
            {
                if (i > Lives) inWorldLives[i].SetActive(false);
            }
        }

        if(Lives < -1)// Should be dead
        {
            OnDeath.Invoke();
        }
    }

    void UpdateBombs()
    {
        //Create lives in layout group
        if (BombsCounter != null && Bomb != null)
        {
            for (int i = 0; i < inWorldBombs.Length; i++)
            {
                if (i > Bombs-1) inWorldBombs[i].SetActive(false);
            }
        }
    }
}
