using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LOL_Movement : MonoBehaviour
{
    NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if(Input.GetButton("Fire1"))
        {
            if(Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }

        }
    }
}
