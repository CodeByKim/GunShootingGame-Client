using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpAndDown : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float result = Mathf.Sin(Time.time);
        float ratio = Random.Range(0.01f, 0.025f);
        result *= ratio;
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y - result, transform.position.z);
        transform.position = newPos;
	}
}
