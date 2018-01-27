using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    GameObject player;
    Vector3 normalModePos;
    [SerializeField] Vector3 insideModePos;
    Camera mainCam;
    [SerializeField] Vector3 offset = new Vector3(2f, 3f, 0);
    bool normalMode = true;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCam = Camera.main;
        normalModePos = mainCam.transform.position;
	}
	
	void FixedUpdate ()
    {
        if(player.GetComponent<PlayerController>().playerstate == PlayerController.state.inside && normalMode == true)
        {
            mainCam.transform.position += offset;
            normalMode = false;
        }
        else
        {
            if (normalMode == false && player.GetComponent<PlayerController>().playerstate != PlayerController.state.inside)
            {
                print("Change view!");
                mainCam.transform.position -= offset;
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
