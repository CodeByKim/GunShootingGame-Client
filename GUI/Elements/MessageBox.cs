using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MessageBox : MonoBehaviour
{
    private UnityAction conmfirmAction;
    private UnityAction closeAction;

    private Text titleText;
    private Text contentText;

    private GameObject attachedTarget;

    public void Initialize()
    {
        this.titleText = transform.Find("Title").GetComponent<Text>();
        this.contentText = transform.Find("Content").GetComponent<Text>();
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

    public void SetTitle(string text)
    {
        this.contentText.text = text;
    }

    public void SetTitle(string title, string content)
    {
        this.titleText.text = title;
        this.contentText.text = content;
    }

    public void OnOK()
    {
        if (this.conmfirmAction != null)
            this.conmfirmAction();

        SoundManager.Instance.PlayUIButtonClick();
        Release();
        Destroy(gameObject);
    }
}
