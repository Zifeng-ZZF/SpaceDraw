using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DrawLine : MonoBehaviour
{
    public GameObject linePrefab;
    public ARSessionOrigin aRSessionOrigin;
    public GameObject lineAnchor;

    private GameObject _currentLine;
    private LineRenderer _lineRenderer;
    private List<Vector3> _drawPositions = new List<Vector3>();
    //private Vector3 _viewPortCenter = new Vector3(0.5f, 0.5f);
    private Vector2 _screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    private Vector3 _newPosition;

    private ARReferencePointManager _pointManager;

    public ARCameraManager arCameraManager;
    public ARRaycastManager arRaycastManager;

    private List<ARRaycastHit> _hit = new List<ARRaycastHit>();

    // Start is called before the first frame update
    void Start()
    {
        _pointManager = aRSessionOrigin.GetComponent<ARReferencePointManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("Touch Began");
                CreateLine();
    }

            if(touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {

                //_screenCenter = Camera.current.ViewportToScreenPoint(_viewPortCenter);
                //_screenCenter = Camera.main.ViewportToScreenPoint(_viewPortCenter);
                _newPosition = Camera.main.ScreenPointToRay(_screenCenter).direction;
                _newPosition.z += 0.01f;
                _newPosition += Camera.main.ScreenToWorldPoint(_screenCenter);


                Debug.Log("newPosition" + _newPosition);

                UpdateLine(_newPosition);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Touch Began");
            CreateLine();
        }

        if (Input.GetMouseButton(0))
        {

        }
    }

    void CreateLine()
    {
        _drawPositions.Clear();
        _lineRenderer = null;

        _currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        _lineRenderer = _currentLine.GetComponent<LineRenderer>();

        //_screenCenter = Camera.main.ViewportToScreenPoint(_viewPortCenter);
        Vector3 startPosition = Camera.main.ScreenPointToRay(_screenCenter).direction;
        startPosition.z += 0.01f;
        startPosition += Camera.main.ScreenToWorldPoint(_screenCenter);

        Vector3 startRot = Camera.main.transform.rotation.eulerAngles;
        Quaternion startRotation = Quaternion.Euler(startRot);

        Pose p = new Pose();
        p.rotation = startRotation;
        p.position = startPosition;
        _pointManager.AddReferencePoint(p);

        lineAnchor.transform.position = startPosition;
        lineAnchor.transform.rotation = startRotation;
        _currentLine.transform.parent = lineAnchor.transform;

        _drawPositions.Add(startPosition);
        _drawPositions.Add(startPosition);

        _lineRenderer.SetPosition(0, _drawPositions[0]);
        _lineRenderer.SetPosition(1, _drawPositions[1]);
    }

    void UpdateLine(Vector3 position)
    {
        _drawPositions.Add(position);
        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, position);
    }
}
