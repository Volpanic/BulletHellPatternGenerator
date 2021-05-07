using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    [Header("Effects")]
    public ParticleSystem GrazeEffect;

    [Header("UI")]
    public HorizontalLayoutGroup LivesCounter;
    public GameObject Life;
    private GameObject[] inWorldLives;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI GrazeText;

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
    }

    private Color clearColor = new Color(1f,1f,1f,0.25f);

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
    }
}
