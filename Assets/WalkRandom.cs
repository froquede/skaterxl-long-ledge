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
    public GameObject agentAnimation;
    private Animation anim;
    float time = 0;
    float floor = 0;
    bool on_floor = false;
    bool already_up = false;
    Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        navmeshagent = agent.GetComponent<UnityEngine.AI.NavMeshAgent>();
        navmeshagent.speed = Random.Range(1.5f, 2.5f);
        destination = GetRandomLocation();
        navmeshagent.SetDestination(destination);
        last_position = Vector3.zero;
        anim = agentAnimation.GetComponent<Animation>();
        camera = (Camera)FindObjectOfType(typeof(Camera));
    }

    // Update is called once per frame
    void Update()
    {
        if (!on_floor)
        {
            if (Vector3.Distance(last_position, agent.transform.position) <= navmeshagent.speed / 20 && navmeshagent.enabled && count >= 24)
            {
                Debug.Log(Vector3.Distance(last_position, agent.transform.position));
                agent.transform.LookAt(new Vector3(camera.transform.position.x, agent.transform.position.y, camera.transform.position.z));
                anim.Play("Yelling");
                navmeshagent.enabled = false;
            }
            else
            {
                if (!anim.isPlaying)
                {
                    anim.Play("Walking");
                    navmeshagent.enabled = true;
                }
            }
        }

        if (navmeshagent.enabled)
        {
            if (count == 12 || count == 24)
            {
                if (camera)
                {
                    float d = Vector3.Distance(agent.transform.position, camera.transform.position);
                    Debug.Log("Actual: " + d);
                    Debug.Log("Last: " + last_distance);

                    if (d <= 2 && (last_distance > d || last_distance < 0f))
                    {
                        last_distance = d;
                        destination = GetRandomLocation();
                    }
                    if (d > 2)
                    {
                        last_distance = -1f;
                    }
                }
            }
            if (count >= 24)
            {
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
        }

        if (!anim.isPlaying && on_floor == true)
        {
            if (already_up == true)
            {
                on_floor = false;
                already_up = false;
                navmeshagent.enabled = true;
                anim.Play("Walking");
            }
            else
            {
                anim.Play("Getting Up");
                already_up = true;
            }
        }

        time++;
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

    private void OnTriggerEnter(Collider collider)
    {
        Rigidbody rb = collider.GetComponent<Rigidbody>();
        if (rb)
        {
            Vector3 localVel = agent.transform.InverseTransformDirection(rb.velocity);
            if (localVel.x > 0.1f || localVel.y > 0.1f || localVel.z > 0.1f)
            {
                navmeshagent.enabled = false;
                agent.transform.LookAt(new Vector3(rb.transform.position.x, agent.transform.position.y, rb.transform.position.z));
                if (anim) anim.Play("Falling Down");
                on_floor = true;
                floor = time;
            }
        }
    }
}
