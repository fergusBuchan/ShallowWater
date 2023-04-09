using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Vector3 Offset;
    private float mZCoord;
    private Rigidbody rb;
    private Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        int vertexNo = mesh.vertexCount;
        Vector3[] newUV = new Vector3[vertexNo];

        float vx = rb.velocity.x / 100;
        float vy = rb.velocity.y / 100;
        float vz = rb.velocity.z / 100;

        for (int i = 0; i < vertexNo; i++)
        {
            newUV[i] = new Vector3(vx, vy, vz);
        }
        mesh.SetUVs(1, newUV);

        if (Input.GetKeyDown("r"))
        {
            rb.position = new Vector3(0, 5, 0);
        }
    }

    // Update is called once per frame
    void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        Offset = gameObject.transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        rb.velocity = ((GetMouseWorldPos() + Offset) - transform.position) * 10;
    }

    void OnMouseUp()
    {
        rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
    }
    
    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint;
        mousePoint.x = Input.mousePosition.x; 
        mousePoint.y = Input.mousePosition.y;
        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
