//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UI_ELEMENT
{
    MESSAGE_BOX,
    PROMPT_WINDOW
}

public class UIFactory
{
    private Object[] elementPrefabs;

    private static UIFactory instance;
    public static UIFactory Instance
    {
        get
        {
            if (instance == null)
                instance = new UIFactory();

            return instance;
        }
    }

    public UIFactory()
    {
        elementPrefabs = Resources.LoadAll("Prefabs/UI Elements");
    }

    public GameObject Create(UI_ELEMENT element)
    {
        Object prefab = null;
        GameObject createdPrefab = null;

        switch (element)
        {
            case UI_ELEMENT.MESSAGE_BOX:
                prefab = System.Array.Find(elementPrefabs, item => item.name == "Message Box 2");
                createdPrefab = GameObject.Instantiate(prefab) as GameObject;
                return createdPrefab;

            case UI_ELEMENT.PROMPT_WINDOW:
                prefab = System.Array.Find(elementPrefabs, item => item.name == "Prompt Window 2");
                createdPrefab = GameObject.Instantiate(prefab) as GameObject;
                return createdPrefab;
        }

        return null;
    }
}
