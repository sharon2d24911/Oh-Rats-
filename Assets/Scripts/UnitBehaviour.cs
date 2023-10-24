﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviour : MonoBehaviour
{
    [System.Serializable]
    public struct Animations
    {
        public string animation;
        public List<Sprite> BaseAnimation;
        public List<Sprite> AttackAnimation;
        public List<Sprite> HealthAnimation;
        public List<Sprite> SpeedAnimation;
    }

    public GameObject projectile;
    private GameObject myProjectile;
    public Transform ProjectileOrigin;
    public float cooldown;
    private bool canShoot;
    private Camera MainCamera;
    private Dictionary<Vector2, GameObject> unitPositions;
    [HideInInspector] public bool defending = false;
    [HideInInspector] public bool placed = false;
    public float range;
    public float health;
    public float damageTime;
    [HideInInspector] public float projAddAttack = 0;
    [HideInInspector] public float projAddSpeed = 0;
    public LayerMask projectileMask;
    private GameObject target;
    private GameObject unit;

    //======Animation Stuff=========
    public float frameRate = 4f;
    private float animTimer;
    public float animTimeMax; //max seconds per frame. concept taken from lab 4
    public Animations[] animations;
    int animIndex = 0;
    private SpriteRenderer sprite;
    private SpriteRenderer attackLayer;
    private SpriteRenderer healthLayer;
    private SpriteRenderer speedLayer;
    [HideInInspector] public int healthBoost, speedBoost, attackBoost;
    //======Animation Stuff=========


    void Start()
    {
        Debug.Log("Im alive!");

        Debug.Log(healthBoost);
        Debug.Log(speedBoost);
        Debug.Log(attackBoost);

        Invoke("ResetCooldown", cooldown);
        unit = gameObject;
        sprite = unit.GetComponent<SpriteRenderer>();
        MainCamera = Camera.main;
        unitPositions = MainCamera.GetComponent<DragCombination>().filledPositions;
        attackLayer = unit.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
        healthLayer = unit.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>();
        speedLayer = unit.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>();
        animTimeMax = animTimeMax / frameRate;

    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, range, projectileMask);
        Idle();
        if (defending)
        {
            Shoot();
        }
        if (health <= 0)
        {
            unitPositions.Remove(unit.transform.position);
            Destroy(unit); //kills the unit
        }
    }

    void Idle()
    {
        int animFrames = animations[0].BaseAnimation.Count; //should be conistent across all animations, otherwise everything will look wonky
        animTimer += Time.deltaTime;

       if(animTimer > animTimeMax)
        {
            animTimer = 0;
            if (animIndex < animFrames - 1)
           {
               animIndex++;
           }
           else
           {
               animIndex = 0;
           }

            //assuming Idle is the first array in "Animations"
            //-->cycles through all layers of animations 

        sprite.sprite = animations[0].BaseAnimation[animIndex]; 
        attackLayer.sprite = animations[0].AttackAnimation[animIndex + (animFrames * (attackBoost/2) ) ]; //adjusts starting point of anim index depending on the boost of each stat
        speedLayer.sprite = animations[0].SpeedAnimation[animIndex + (animFrames * (speedBoost/2) ) ];
        healthLayer.sprite = animations[0].HealthAnimation[animIndex + (animFrames * (healthBoost/2) ) ];

        sprite.color += new Color(0, 0, 0, 1);
        //depending on stats of the unit, change appearance of layers
        attackLayer.color += new Color(0, 0, 0, (attackBoost > 0 ? 1 : 0));  //use of ternary operator to determine whether each respective layer should be visible or not
        speedLayer.color += new Color(0, 0, 0, (speedBoost > 0 ? 1 : 0));
        healthLayer.color += new Color(0, 0, 0, (healthBoost > 0 ? 1 : 0));

        }
    }

    void ResetCooldown()
    {
        canShoot = true;
    }


    public IEnumerator takeDamage(float dmgAmount)
    {
        
        health -= dmgAmount;
        sprite.color = Color.red;
        Debug.Log("DAMAGE UNIT");
        yield return new WaitForSeconds(damageTime / 5);
        if (unit != null && sprite != null)
        {
            sprite.color = Color.white;
        }
    }

    void Shoot()
    {
        if (!canShoot)
           return;
        canShoot = false;
        Invoke("ResetCooldown", cooldown);
        myProjectile = Instantiate(projectile, ProjectileOrigin.position, Quaternion.identity);
        
        // Projectile update to match the unit's boosted stats
        myProjectile.GetComponent<ProjectileScript>().attack += projAddAttack;
        myProjectile.GetComponent<ProjectileScript>().speed += projAddSpeed;

    }
}
