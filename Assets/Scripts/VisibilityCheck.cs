using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityCheck : MonoBehaviour
{

    [SerializeField] private LayerMask BlockLayers;

    private Camera cam;

    private Collider objCollider;

    private Plane[] planes;

    void Start()
    {
        cam = Camera.main;
        objCollider = GetComponent<Collider>();
    }

    public int IsVisible()
    {
        planes = GeometryUtility.CalculateFrustumPlanes(cam);

        if (GeometryUtility.TestPlanesAABB(planes, objCollider.bounds))
        {
            if (Physics.Linecast(transform.position, cam.transform.position, out RaycastHit hit, BlockLayers))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            //Debug.Log("Nothing has been detected");
        }

        return 2;

    }
}
