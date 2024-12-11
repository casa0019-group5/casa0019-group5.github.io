using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;


[RequireComponent(typeof(ARRaycastManager))]
public class tapToPlace : MonoBehaviour
{
    public GameObject gameObjectToInstantiate; //the Prefab GameObject to instantiate in the AR environment. To be added in the inspector window
    private GameObject spawnedObject; //the Prefab Instantiate in the scene. Used internally by the script 
    private ARRaycastManager _arRaycastManager; //part of the XROrigin

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public float timeThreshold = 0.5f; //User need to tap and hold the finger for at least 0.5 sec to create the content
    public bool isTouching = false;

    //Event design to fire when content is created
    public delegate void ContentVisibleDelegate();
    public event ContentVisibleDelegate _contentVisibleEvent;

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }

    public bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        Debug.Log("Hello");
        if (Touchscreen.current.primaryTouch.press.isPressed)
        {
            isTouching = true;
            touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }
        touchPosition = default;
        isTouching = false;
        timeThreshold = 0;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTouching == true)
        {
            timeThreshold -= Time.deltaTime;
        }

        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (_arRaycastManager.Raycast(touchPosition, hits, trackableTypes: TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            if (timeThreshold < 0)
            {
                if (spawnedObject == null)
                {
                    spawnedObject = Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation);
                    _contentVisibleEvent(); //fire the event
                }
                else
                {
                    spawnedObject.transform.position = hitPose.position;
                }
            }
        }
    }
}