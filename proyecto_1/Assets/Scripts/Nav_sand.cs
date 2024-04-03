using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Nav_TerrainModifier : MonoBehaviour
{
    private NavMeshModifier _meshSurface;
    // Start is called before the first frame update
    void Start()
    {
        _meshSurface = GetComponent<NavMeshModifier>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Enter");
        // Compruebo que lo que ha entrado es un agente
        if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            // Compruebo que el agente se ve afectado por este tipo de terreno
            if (_meshSurface.AffectsAgentType(agent.agentTypeID))
            {
                agent.speed /= NavMesh.GetAreaCost(_meshSurface.area);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Debug.Log("Exit");
        // Compruebo que lo que ha salido es un agente
        if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            // Compruebo que el agente se ve afectado por este tipo de terreno
            if (_meshSurface.AffectsAgentType(agent.agentTypeID))
            {
                agent.speed *= NavMesh.GetAreaCost(_meshSurface.area);
            }
        }
    }
}
