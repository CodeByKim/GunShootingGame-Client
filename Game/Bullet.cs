using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour 
{
    private GamePlayer _owner;

    public GamePlayer Owner
    {
        get { return _owner; }
        set { _owner = value; }
    }

	void Start () 
    {
        Destroy(gameObject, 1.5f);
	}
		
	void Update () 
    {
        transform.Translate(transform.forward * Time.deltaTime * 15, Space.World);
	}

    private void OnTriggerEnter(Collider other)
    {
        switch(other.tag)
        {
            case "Enemy":
                other.GetComponent<Enemy>().Hit(Owner);
                Destroy(gameObject);

                break;
            case "Obstacle":
                Destroy(gameObject);

                break;
        }
    }
}
