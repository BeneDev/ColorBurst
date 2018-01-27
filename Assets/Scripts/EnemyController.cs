﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    [SerializeField] Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    Material mat;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        GotoNextPoint();
        mat = transform.Find("Enemy").transform.Find("MainBody").GetComponent<Renderer>().material;
        //mat = GetComponentInChildren<Renderer>().material;
    }


    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
        {
            return;
        }

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }

    void SetOutline(float thickness)
    {
        mat.SetFloat("_OutlineWidth", thickness);
    }

    void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GotoNextPoint();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerTarget")
        {
            print("Set the outline");
            SetOutline(0.1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SetOutline(0);
    }

}
