using UnityEngine;
using System.Collections;
using System;

public class MeasurerScript : MonoBehaviour {
    Vector3 startPoint;
    Vector3 normal;
    bool isMeasuring;

    TextMesh textMesh;

    GameObject headBlock;
    GameObject tailBlock;
    GameObject bar;

	// Use this for initialization
	void Start () {
        isMeasuring = false;
        textMesh = transform.Find("Label").gameObject.GetComponent<TextMesh>();
        headBlock = transform.Find("HeadBlock").gameObject;
        tailBlock = transform.Find("TailBlock").gameObject;
        bar = transform.Find("Bar").gameObject;
	}

    public void StartMeasuring(Vector3 startPoint, Vector3 normal, Vector3 headPosition)
    {
        this.startPoint = startPoint;
        this.normal = normal;
        this.isMeasuring = true;

        //var lookAtPoint = GetLookAtPoint(startPoint, normal, headPosition);

        this.transform.position = startPoint;
        //this.transform.LookAt(lookAtPoint);
        //this.transform.Rotate(0, 180, 0);
        this.transform.Rotate(normal);
        // Show the arrow
    }

    //private Vector3 GetLookAtPoint(Vector3 startPoint, Vector3 normal, Vector3 headPosition)
    //{
    //    Ray ray = new Ray(headPosition, -normal);
    //    Plane p = new Plane(normal, startPoint);
    //    float distance;
    //    p.Raycast(ray, out distance);
    //    return ray.GetPoint(distance);
    //}

    public void StopMeasuring()
    {
        isMeasuring = false;
        // Hide the arrow
    }

    public bool IsMeasuring()
    {
        return this.isMeasuring;
    }

    public void UpdatePoints(Vector3 headPosition, Vector3 gazeDirection)
    {
        const float lengthFactor = 0.5f;       // The ratio of model size to measure units
        var endPoint = GetIntersectionPoint(headPosition, gazeDirection, startPoint, normal);

        float distance = (endPoint - startPoint).magnitude;
        var position = headBlock.transform.localPosition;
        position.y = transform.InverseTransformPoint(endPoint).y;
        headBlock.transform.localPosition = position;

        var scale = bar.transform.localScale;
        scale.y = Math.Abs(distance * lengthFactor);
        bar.transform.localScale = scale;
        bar.transform.position = (endPoint + startPoint) / 2;

        textMesh.text = String.Format(" {0:F2}m", distance);
        textMesh.transform.position = endPoint;
        textMesh.transform.LookAt(headPosition);
        textMesh.transform.Rotate(0, 180, 0);
    }

    private Vector3 GetIntersectionPoint(Vector3 headPosition, Vector3 gazeDirection, Vector3 startPoint, Vector3 normal)
    {
        Ray ray = new Ray(startPoint, normal);
        Vector3 normalSideWay = Vector3.Cross(gazeDirection, new Vector3(0, 1, 0));
        Plane plane = IsVerticalVector(normal) ? new Plane(Vector3.Cross(normalSideWay, gazeDirection), headPosition) : new Plane(normalSideWay, headPosition);
        float distance;
        plane.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    private bool IsVerticalVector(Vector3 vector)
    {
        return AngleBetween(vector, new Vector3(0, 1, 0)) < Math.PI / 4 || AngleBetween(vector, new Vector3(0, -1, 0)) < Math.PI / 4;
    }

    private float AngleBetween(Vector3 vec1, Vector3 vec2)
    {
        return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
    }
}
