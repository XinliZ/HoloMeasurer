using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Windows.Speech;

public class MainScript : MonoBehaviour
{

    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    MeasurerScript measurer;
    GameObject cursor;

    // Use this for initialization
    void Start()
    {
        measurer = transform.Find("Pointer").gameObject.GetComponent<MeasurerScript>();
        cursor = transform.Find("Cursor").gameObject;

        keywords.Add("Measure", () =>
        {
            // Start measuring
            // Pick the plane
            // Set the measure pointer
            var headPosition = Camera.main.transform.position;
            var gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;

            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
            {
                // TODO: If the normal doesn't work, use the game object
                measurer.StartMeasuring(hitInfo.point, hitInfo.normal, headPosition);
            }
            GetComponent<AudioSource>().Play();
        });

        keywords.Add("Stop", () =>
        {
            // Turn off the measure
            measurer.StopMeasuring();
            GetComponent<AudioSource>().Play();
        });

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();

    }

    // Update is called once per frame
    void Update()
    {
        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        
        // If the raycast hit a hologram...
        // Display the cursor mesh.
        if (measurer.IsMeasuring())
        {
            measurer.UpdatePoints(headPosition, gazeDirection);
        }
        else
        {
            RaycastHit hitInfo;

            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
            {
                cursor.transform.position = hitInfo.point;
                this.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            }
        }
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
}
