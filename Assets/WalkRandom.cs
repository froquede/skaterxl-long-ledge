using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkRandom : MonoBehaviour
{
    public GameObject agent;
    public Vector3 destination;
    private UnityEngine.AI.NavMeshAgent navmeshagent;
    public UnityEngine.AI.NavMeshTriangulation navMeshData;
    public int count = 0;
    public Vector3 last_position;
    public float last_distance = -1f;
    // Start is called before the first frame update
    void Start()
    {
        navmeshagent = agent.GetComponent<UnityEngine.AI.NavMeshAgent>();
        navmeshagent.speed = Random.Range(1.5f, 2.5f);
        destination = GetRandomLocation();
        navmeshagent.SetDestination(destination);
        last_position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (count >= 24)
        {
            Camera camera = (Camera)FindObjectOfType(typeof(Camera));
            if (camera)
            {
                float d = Vector3.Distance(agent.transform.position, camera.transform.position);

                if (d <= 8f && (last_distance > d || last_distance < 0f))
                {
                    last_distance = d;
                    destination = GetRandomLocation();
                }
            }

            if (navmeshagent.remainingDistance <= navmeshagent.stoppingDistance)
            {
                destination = GetRandomLocation();
            }
            else
            {
                if (Vector3.Distance(agent.transform.position, last_position) <= .1f)
                {
                    destination = GetRandomLocation();
                }
            }
            last_position = agent.transform.position;
            count = 0;
        }
        navmeshagent.SetDestination(destination);
        count++;
    }

    Vector3 GetRandomLocation()
    {
        navMeshData = UnityEngine.AI.NavMesh.CalculateTriangulation();
        int t = Random.Range(0, navMeshData.indices.Length - 3);
        Vector3 point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]], navMeshData.vertices[navMeshData.indices[t + 1]], Random.value);
        Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t + 2]], 1f);

        return point;
    }
}
