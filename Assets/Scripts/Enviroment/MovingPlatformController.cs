using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    public float speed = 5f;
    public Vector3 direction = Vector3.right;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    public void ParentPlayer(Collider other)
    {
        Debug.Log("Try Parent: " + other.gameObject.name);

        if (other.gameObject.tag != "Player") { return; }

        other.transform.parent = transform;

        Debug.Log("Parented Player!");
    }

    public void UnParentPlayer(Collider other)
    {
        if (other.gameObject.tag != "Player") { return; }

        other.transform.parent = null;

        Debug.Log("UnnParented Player");
    }
}
