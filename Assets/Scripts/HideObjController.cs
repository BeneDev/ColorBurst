using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObjController : MonoBehaviour {

    public bool shineOn = false;
    Material mat;

	// Use this for initialization
	void Start () {
        mat = transform.Find("Body").GetComponentInChildren<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void SetOutline(float thickness)
    {
        mat.SetFloat("_Outline", thickness);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EnemyTarget")
        {
            shineOn = true;
        }
        else if(other.gameObject.tag == "PlayerTarget")
        {
            SetOutline(0.4f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (shineOn == true)
        {
            shineOn = false;
        }
        else
        {
            SetOutline(0);
        }
    }
}
