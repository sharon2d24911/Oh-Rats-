﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prop : MonoBehaviour
{
    public GameObject background;
    public Texture2D interactCursor;
    public Texture2D defaultCursor;
    public GameObject leftButton;
    public GameObject rightButton;
    private List<Sprite> flipthrough = new List<Sprite>();
    private GameObject gameHandler;
    private GameObject newspaper;
    private int i;
    [HideInInspector] public bool inspecting;
    private float fadeDuration = 2f;

    // Start is called before the first frame update
    void Start()
    {
        gameHandler = FindObjectOfType<GameHandler>().gameObject;
        i = 0;
        inspecting = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Determining which buttons to disable
        if (leftButton.activeInHierarchy && rightButton.activeInHierarchy)
        {
            if (i <= 0)
                leftButton.GetComponent<Button>().interactable = false;
            else
                leftButton.GetComponent<Button>().interactable = true;
            if (i >= flipthrough.Count - 1)
                rightButton.GetComponent<Button>().interactable = false;
            else
                rightButton.GetComponent<Button>().interactable = true;
        }
    }

    // Show the left and right buttons if there are items to flip through
    public void SetButtonsActive(List<Sprite> flip, GameObject prop)
    {
        flipthrough = flip;
        newspaper = prop;
        i = flipthrough.Count - 1;
        leftButton.SetActive(true);
        rightButton.SetActive(true);
    }

    // Hide buttons again once player clicks back out of prop
    public void SetButtonsInactive()
    {
        leftButton.SetActive(false);
        rightButton.SetActive(false);
    }

    // Flip to the previous newspaper
    public void FlipBack()
    {
        i -= 1;
        newspaper.GetComponent<SpriteRenderer>().sprite = flipthrough[i];
    }

    // Flip to the next newspaper
    public void FlipForward()
    {
        i += 1;
        newspaper.GetComponent<SpriteRenderer>().sprite = flipthrough[i];
    }

    public void TransitionScenes(GameObject currentScene, GameObject nextScene)
    {
        StartCoroutine(Fade(currentScene, nextScene));
    }
    
    public IEnumerator Fade(GameObject currentScene, GameObject nextScene)
    {
        nextScene.SetActive(true);
        Color fullAlpha = new Color(1, 1, 1, 1f);
        Color transparent = new Color(1, 1, 1, 0f);
        List<SpriteRenderer> currentChildren = new List<SpriteRenderer>();
        List<SpriteRenderer> nextChildren = new List<SpriteRenderer>();
        foreach (Transform child in currentScene.transform)
        {
            if (child.GetComponent<SpriteRenderer>() != null)
                currentChildren.Add(child.GetComponent<SpriteRenderer>());
        }
        foreach (Transform child in nextScene.transform)
        {
            if (child.GetComponent<SpriteRenderer>() != null)
            {
                child.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0f);
                nextChildren.Add(child.GetComponent<SpriteRenderer>());
            }
        }

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            foreach (SpriteRenderer child in currentChildren)
                child.color = Color.Lerp(fullAlpha, transparent, elapsedTime / fadeDuration);
            foreach (SpriteRenderer child in nextChildren)
                child.color = Color.Lerp(transparent, fullAlpha, elapsedTime / fadeDuration);
            yield return null;
        }
        currentScene.SetActive(false);
    }
}
