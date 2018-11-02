using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGUI : MonoBehaviour 
{
    public static GameGUI Instance;

    private void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    public void ShowGameOver()
    {
        MessageBox messageBox = UIFactory.Instance.Create(UI_ELEMENT.MESSAGE_BOX).GetComponent<MessageBox>();
        messageBox.Initialize();
        messageBox.AttachUIElement(new Vector2(0, 50), gameObject);
        messageBox.SetTitle("Game Over!", "모든 사용자의 게임이 끝날때까지 기다리십시오.");
    }
}
