﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/********************
 * NOT WORKING
 * DON'T USE
 * THANK YOU
 ********************/


public class PropExamineMore : MonoBehaviour
{
    public BoxCollider2D boxCollider;
    [Header("Display size")]
    public Sprite small;
    public Sprite big;
    public Vector3 smallPos;
    public Vector3 smallScale;
    public Vector2 smallBC;
    [Header("Expanded size")]
    public Vector3 bigPos;
    public Vector3 bigScale;
    public Vector2 bigBC;
    public GameObject caption;
    public List<Sprite> flipthrough;

    private Texture2D interactCursor;
    private Texture2D defaultCursor;
    private GameObject background;

    private Vector3 targetPos;
    private Vector3 targetScale;
    private Sprite targetImage;
    private Vector2 targetBCSize;
    private bool isBig;

    private GameObject props;
    private GameObject gameHandler;

    private void Start()
    {
        transform.localPosition = smallPos;
        transform.localScale = smallScale;
        targetScale = transform.localScale;
        GetComponent<SpriteRenderer>().sprite = small;
        isBig = false;
        props = FindObjectOfType<Prop>().gameObject;
        interactCursor = props.GetComponent<Prop>().interactCursor;
        defaultCursor = props.GetComponent<Prop>().defaultCursor;
        background = props.GetComponent<Prop>().background;
        gameHandler = FindObjectOfType<GameHandler>().gameObject;
        if (caption != null)
            caption.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        // Toggle the target scale on mouse click
        if (Time.timeScale != 0f)
            ToggleSize();

        if (transform.localScale != targetScale)
        {
            // Transform to the new position/rotation/scale based on isBig bool
            transform.position = targetPos;
            transform.localScale = targetScale;
            GetComponent<SpriteRenderer>().sprite = targetImage;
            boxCollider.size = targetBCSize;

            // Set layering, visibility for background, and prop captions (also flip buttons if newspaper prop specifically)
            if (isBig)
            {
                GetComponent<SpriteRenderer>().sortingOrder = 50;
                background.GetComponent<SpriteRenderer>().sortingOrder = 49;
                background.SetActive(true);
                if (caption != null)
                {
                    caption.transform.position = new Vector3(transform.position.x, (transform.position.y - 8f), 1f);
                    caption.SetActive(true);
                }
                if (flipthrough.Count > 0)
                {
                    props.GetComponent<Prop>().SetButtonsActive(flipthrough, this.gameObject);
                }
            }
            else
            {
                if (caption != null)
                    caption.SetActive(false);
                GetComponent<SpriteRenderer>().sortingOrder = 0;
                background.GetComponent<SpriteRenderer>().sortingOrder = 0;
                background.SetActive(false);
                if (flipthrough.Count > 0)
                    props.GetComponent<Prop>().SetButtonsInactive();
            }
        }
    }

    void OnMouseEnter()
    {
        if (Time.timeScale != 0f && !gameHandler.GetComponent<DragCombination>().trashMode)
            Cursor.SetCursor(interactCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    void OnMouseExit()
    {
        if (!gameHandler.GetComponent<DragCombination>().trashMode)
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    private void ToggleSize()
    {
        // Toggle the "isBig" bool
        isBig = !isBig;
        // If isBig, first condition (big pos/scale), if !isBig, second condition (small pos/scale)
        Vector3 position = isBig ? bigPos : smallPos;
        Vector3 scale = isBig ? bigScale : smallScale;
        Sprite image = isBig ? big : small;
        Vector2 BC = isBig ? bigBC : smallBC;
        // Set the target for next click
        targetPos = position;
        targetScale = scale;
        targetImage = image;
        targetBCSize = BC;
    }
}
