﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Animator animator;
    private string sceneToLoad;
    //public Texture2D basic;

    void Start()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
        //Time.timeScale = 1f;
    }

    // Using animator to fade scene out to black
    public void FadeToScene(string sceneName)
    {
        Time.timeScale = 1f;
        sceneToLoad = sceneName;
        animator.SetTrigger("FadeOut");
        Debug.Log("scane name"+SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().PlaySFX("StartButton", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["StartButton"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["StartButton"][1]);
            StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeOut("TitleWop", "none", "none", "none", 1, 0));
            string[] tracks = { "ChewTorial", "none", "none", "none" };
            float[] volumes = { 1, 1, 1, 1 };
            float[] speeds = { 1, 1, 1, 1 };
            StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeIn(tracks, 1, volumes, speeds));
        }
        else if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeOut("ChewTorial", "none", "none", "none", 1, 0));
        }
        else
        {
            StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeOut("BassyMain", "none", "none", "none", 1, 0)); // Fade out music
        }
        
    }

    // Animation event on the completion of fading out to call scene
    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
