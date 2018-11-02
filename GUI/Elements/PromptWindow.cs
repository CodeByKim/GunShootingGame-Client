using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PromptWindow : MonoBehaviour
{
    private UnityAction conmfirmAction;
    private UnityAction closeAction;
    
    private Text titleText;
    private InputField input;

    private GameObject attachedTarget;

    public void Initialize()
    {
        this.titleText = transform.Find("Title").GetComponent<Text>();
        this.input = transform.Find("InputField").GetComponent<InputField>();
    }

    public void Release()
    {
        this.attachedTarget.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void AttachUIElement(Vector2 pos, GameObject attachedTarget)
    {
        GetComponent<RectTransform>().SetParent(GameObject.Find("GUI").transform.Find("Generate UI Elements"));
        GetComponent<RectTransform>().anchoredPosition = pos;
        this.attachedTarget = attachedTarget;

        this.attachedTarget.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void AddConfirmEventFunc(UnityAction action)
    {
        this.conmfirmAction = action;
    }

    public void AddCloseEventFunc(UnityAction action)
    {
        this.closeAction = action;
    }

    public void SetTitle(string text)
    {
        this.titleText.text = text;
    }

    public string GetInputText()
    {        
        return this.input.text;
    }

    public void OnOK()
    {
        if(this.conmfirmAction != null)
            this.conmfirmAction();

        SoundManager.Instance.PlayUIButtonClick();
        Release();
        Destroy(gameObject);
    }

    public void OnClose()
    {
        if (this.closeAction != null)
            this.closeAction();

        SoundManager.Instance.PlayUIButtonClick();
        Release();
        Destroy(gameObject);
    }
}
