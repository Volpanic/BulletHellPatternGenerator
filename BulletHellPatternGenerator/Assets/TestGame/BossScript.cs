using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossScript : MonoBehaviour
{
    [System.Serializable]
    public struct HealthbarBand
    {
        public BulletHellPattern Pattern;
        public int HPAmount;
    }

    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float transitionDuration = 1;
    private float transitionTimer = 0;
    private bool transitioning = false;

    public BH_BulletHellPatternGenerator Generator;
    public int MaxHp = 0;
    public int CurrentHp = 0;
    public GameObject PointPaper;

    public HealthbarBand[] AttackBands;

    private int bandHpAmount = 0;
    private int currentBand = 0;

    public UnityEvent OnDeath;

    private bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < AttackBands.Length; i++)
        {
            MaxHp += AttackBands[i].HPAmount;

            if (i == 0)
            {
                bandHpAmount = AttackBands[i].HPAmount;
                Generator.Patterns[0] = AttackBands[i].Pattern;
            }
        }

        CurrentHp = MaxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if(transitioning)
        {
            transitionTimer += Time.deltaTime;

            transform.position = Vector3.Lerp(startPosition, targetPosition, transitionTimer / transitionDuration);

            if(transitionTimer >= transitionDuration)
            {
                transitioning = false;
                transitionTimer = 0;
                transform.position = targetPosition;
            }
        }
    }

    public void MoveToPoint(Vector3 targetPos, float durationInSeconds)
    {
        targetPosition = targetPos;
        transitionDuration = durationInSeconds;
        transitionTimer = 0;
        transitioning = true;
        startPosition = transform.position;
    }

    public void Hurt(int Amount)
    {
        bandHpAmount -= Amount;
        CurrentHp -= Amount;

        if (dead) return;

        //Whiles in case a bands hp is 0, or the player does a lot of damage
        while(bandHpAmount <= 0)
        {
            currentBand++;
            Debug.Log("Hurt");

            if(Input.GetKeyDown(KeyCode.R))
            {
                bandHpAmount = 128;
            }

            Generator.ClearBullets(CreateScorePaper);

            if (currentBand < AttackBands.Length)
            {
                int newAmount = AttackBands[currentBand].HPAmount + bandHpAmount; // Adds the negatives if below 0
                bandHpAmount = newAmount;
                Generator.Patterns[0] = AttackBands[currentBand].Pattern;
            }
            else
            {
                dead = true;
                OnDeath.Invoke();
                break;
            }
        }
    }

    public void CreateScorePaper(Transform from)
    {
        Instantiate(PointPaper, from.position, Quaternion.identity).GetComponent<PointPaper>().Target = Generator.Target;
    }
}
