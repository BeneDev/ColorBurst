using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    GameObject player;
    Vector3 normalModePos;
    Camera mainCam;
    [SerializeField] Vector3 offset = new Vector3(0f, 3f, 0);
    bool normalMode = true;
    float rendSize;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCam = Camera.main;
        normalModePos = mainCam.transform.position;
	}
	
	void FixedUpdate ()
    {
        if(player.GetComponent<PlayerController>().playerstate == PlayerController.state.inside && normalMode == true)
        {
            print("Change into Inside view");
            rendSize = player.GetComponent<PlayerController>().currentlyInside.GetComponent<Renderer>().bounds.size.y;
            mainCam.transform.position += offset * rendSize;
            normalMode = false;
        }
        else
        {
            if (normalMode == false && player.GetComponent<PlayerController>().playerstate != PlayerController.state.inside)
            {
                print("Change into outside view!");
                mainCam.transform.position -= offset * rendSize;
                normalMode = true;
            }
        }
	}

    private void LateUpdate()
    {
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
    }
}
