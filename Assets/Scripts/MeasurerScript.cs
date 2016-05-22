using UnityEngine;
using System.Collections;
using System;

public class MeasurerScript : MonoBehaviour {
    Vector3 startPoint;
    Vector3 normal;
    bool isMeasuring;

    TextMesh textMesh;
    Plane hitPlane;

	// Use this for initialization
	void Start () {
        isMeasuring = false;
        textMesh = transform.Find("Label").gameObject.GetComponent<TextMesh>();
        hitPlane = transform.Find("HitPlane").gameObject.GetComponent<Plane>();
	}

    public void StartMeasuring(Vector3 startPoint, Vector3 normal, Vector3 headPosition)
    {
        this.startPoint = startPoint;
        this.normal = normal;
        this.isMeasuring = true;

        var lookAtPoint = GetLookAtPoint(startPoint, normal, headPosition);

        this.transform.position = startPoint;
        this.transform.LookAt(lookAtPoint);
        this.transform.Rotate(0, 180, 0);
        //this.transform.Rotate(normal);
        // Show the arrow
    }

    private Vector3 GetLookAtPoint(Vector3 startPoint, Vector3 normal, Vector3 headPosition)
    {
        Ray ray = new Ray(headPosition, -normal);
        Plane p = new Plane(normal, startPoint);
        float distance;
        p.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    public void StopMeasuring()
    {
        isMeasuring = false;
        // Hide the arrow
    }

    public bool IsMeasuring()
    {
        return this.isMeasuring;
    }

    public Vector3 UpdatePoints(Vector3 headPosition, Vector3 gazeDirection)
    {
        var rawPoint = GetRayToPlanePoint(headPosition, gazeDirection, hitPlane);
        var point = this.transform.TransformPoint(rawPoint);

        float distance = 3;    // point.y - startPoint.y;
        var scale = this.transform.localScale;
        scale.y = distance;
        this.transform.localScale = scale;
        textMesh.text = distance.ToString();
        return rawPoint;
    }

    public Vector3 GetRayToPlanePoint(Vector3 headPosition, Vector3 gazeDirection, Plane hitPlane)
    {
        float distanceToHitPlane;
        var ray = new Ray(headPosition, gazeDirection);
        hitPlane.Raycast(ray, out distanceToHitPlane);
        return ray.GetPoint(distanceToHitPlane);
    }

}
