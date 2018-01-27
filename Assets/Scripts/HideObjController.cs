using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObjController : MonoBehaviour {

    public bool shineOn = false;
    Material mat;

	// Use this for initialization
	void Start () {
        mat = GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void SetOutline(float thickness)
    {
        mat.SetFloat("_OutlineWidth", thickness);
    }

    private void OnTriggerEnter(Collider other)
    {
        shineOn = true;
    }

    private void OnTriggerExit(Collider other)
    {
        shineOn = false;
    }

}
