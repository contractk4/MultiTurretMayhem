﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class LevelSelectController : MonoBehaviour {

    // Use this for initialization
    public GameObject levelNumParent;
    private List<UIScroller> levels;
    private MenuController menuController;
    private GraphicColorLerp blackPanel;
    public int levelIndex;
    public GraphicColorLerp leftArrow;
    public GraphicColorLerp rightArrow;
    public AudioClip soundOnMoveFail;
    public AudioClip soundOnMoveOK;
    private float inputDisabledTimer = 0;
    public float delayAfterInput;       //After pressing left or right, disable additional input to allow text to scroll
    private EventSystem eventSystem;
    private bool levelTransition = false;
	void Start () {
        menuController = GameObject.Find("MainMenuController").GetComponent<MenuController>();
        blackPanel = GameObject.Find("BlackPanel").GetComponent<GraphicColorLerp>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        levels = getScollers(levelNumParent);
        levelIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (inputDisabledTimer > 0)
        {
            inputDisabledTimer -= Time.deltaTime;
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                scrollLeft();
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                scrollRight();
            }
        }
        if (!levelTransition && Input.GetKeyDown(KeyCode.Escape))
            backPress();
    }
    private List<UIScroller> getScollers(GameObject obj)
    {
        List<UIScroller> children = new List<UIScroller>();         //get all children of this gameobject
                                                                    
        foreach (Transform child in obj.transform)
        {
            children.Add(child.gameObject.GetComponent<UIScroller>());
        }
        return children;
    }
    public void scrollLeft()
    {
        if(levelIndex > 0)
        {
            levels[levelIndex--].hideUI(-1);
            levels[levelIndex].revealUI(-1);
            menuController.menuEffects.clip = soundOnMoveOK;
        }
        else
        {
            menuController.menuEffects.clip = soundOnMoveFail;
        }
        leftArrow.startColorChange();

        inputDisabledTimer = delayAfterInput;

        menuController.menuEffects.Play();
    }
    public void scrollRight()
    {
        if(levelIndex < levels.Count-1)
        {
            levels[levelIndex++].hideUI(1);
            levels[levelIndex].revealUI(1);
            menuController.menuEffects.clip = soundOnMoveOK;
        }
        else
        {
            menuController.menuEffects.clip = soundOnMoveFail;
        }
        rightArrow.startColorChange();

        inputDisabledTimer = delayAfterInput;
        menuController.menuEffects.Play();
    }
    public void playPress()
    {
        StartCoroutine(HelperFunctions.negInterpolateSound(menuController.musicSource, 2f));
        StartCoroutine(loadLevel());
        eventSystem.SetSelectedGameObject(null);

    }
    private IEnumerator loadLevel()
    {
        levelTransition = true;
        LevelNumber.setLevel(levelIndex);
        blackPanel.gameObject.GetComponent<Image>().enabled = true;
        blackPanel.startColorChange();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Campaign");
    }
    public void backPress()
    {
        levels[levelIndex].hideImmediately();
        menuController.setMenuState(1);
    }
    /*
    public void navigateSound()
    {
        menuController.menuEffects.clip = soundOnMove;
        menuController.menuEffects.Play();
    }
    */

}
