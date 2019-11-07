using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.UI;
using System;

public class ARTapToPlace : MonoBehaviour
{
    public GameObject placementIndicator;
    public Text text;

    private ARSessionOrigin _arOrigin;
    private Pose _placementPose;
    private bool _placementPoseIsValid = false;
    private ARRaycastManager _arRay;

    private void Start()
    {
        //_arOrigin = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
        //_arRay = GameObject.Find("AR Session Origin").GetComponent<ARRaycastManager>();

        _arOrigin = FindObjectOfType<ARSessionOrigin>();
        _arRay = FindObjectOfType<ARRaycastManager>();
    }

    private void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }

    /// <summary>
    /// update the pose of the placement per frame
    /// </summary>
    private void UpdatePlacementPose()
    {
        //viewport is from (0,0) at the bottom left to (1,1) at the top right. So the center is (0.5, 0.5)
        if(Camera.current != null)
        {
            Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

            //the list to store the hits that happen in the current frame
            List<ARRaycastHit> hitsResult = new List<ARRaycastHit>();

            //ray casted from the center of the screen & return the hits to the hitsResultList
            _arRay.Raycast(screenCenter, hitsResult, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

            //if there is any hit, perform update
            _placementPoseIsValid = hitsResult.Count > 0;
            if (_placementPoseIsValid)
            {
                text.text = "true";
                _placementPose = hitsResult[0].pose;
                _placementPose.position.y += 0.05f;

                //let the indicator point to camera's forward direction
                Vector3 cameraForward = Camera.current.transform.forward;
                Vector3 normalizedCameraForward = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                _placementPose.rotation = Quaternion.LookRotation(normalizedCameraForward);
            }
        }
    }

    /// <summary>
    /// Update the indicator (aka the visual effect) to the new pose per frame
    /// </summary>
    private void UpdatePlacementIndicator()
    {
        if (_placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(_placementPose.position, _placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }
}
