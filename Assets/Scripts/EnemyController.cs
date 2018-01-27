using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    [SerializeField] Transform[] points;
    [SerializeField] Animation anim;
    private int destPoint = 0;
    private NavMeshAgent agent;
    Material mat;
    public bool hijacked = false;
    GameObject player;
    color ownColor;
    public int accessColor;
    public MeshCollider attackField;
    public enum color
    {
        orange,
        green,
        red,
        blue
    }

    private void Awake()
    {
        attackField = transform.Find("Enemy").GetComponentInChildren<MeshCollider>();
        mat = transform.Find("Enemy").transform.Find("An@Walking").transform.Find("MainBody").GetComponent<Renderer>().material;
        player = GameObject.FindGameObjectWithTag("Player");
        if (transform.Find("Enemy").transform.Find("KEGEL!").gameObject.layer == 8)
        {
            ownColor = color.orange;
            accessColor = 11;
        }
        else if (transform.Find("Enemy").transform.Find("KEGEL!").gameObject.layer == 9)
        {
            ownColor = color.green;
            accessColor = 10;
        }
        else if (transform.Find("Enemy").transform.Find("KEGEL!").gameObject.layer == 10)
        {
            ownColor = color.red;
            accessColor = 9;
        }
        else if (transform.Find("Enemy").transform.Find("KEGEL!").gameObject.layer == 11)
        {
            ownColor = color.blue;
            accessColor = 8;
        }
    }

    IEnumerator Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animation>();
        anim.Play(anim.clip.name);
        yield return new WaitForSeconds(anim.clip.length);

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        GotoNextPoint();
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
        mat.SetFloat("_Outline", thickness);
    }

    void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (hijacked == false)
        {
            GetComponent<Animator>().enabled = true;
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GotoNextPoint();
            }
        }
        else
        {
            GetComponent<Animator>().enabled = false;
            agent.destination = transform.position;
            transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.rotation = player.transform.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerTarget")
        {
            //print("Set the outline");
            SetOutline(0.4f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SetOutline(0);
    }

}
