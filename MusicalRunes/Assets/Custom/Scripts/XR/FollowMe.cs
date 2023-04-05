using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMe : MonoBehaviour
{
    [Tooltip("The transform to rotate along with")]
    public Transform followTransform;
    public Vector3 offset = new Vector3(0f, -0.25f, 0f);
    public float rotateSpeed = 5f;
    public float movementSmoothing = 0;
    public bool parentToCharacter = false;

    private Vector3 velocity = Vector3.zero;

    /// <summary>
    /// the object to use as a reference
    /// </summary>
    private Transform _followTransform;
    private Transform originalParent;
    private Transform cameraTransform;


    void Start()
    {
        originalParent = transform.parent;
        _followTransform = new GameObject().transform;
        _followTransform.name = "RotationReferenceObject";
        _followTransform.position = transform.position;
        _followTransform.rotation = transform.rotation;

        if (_followTransform)
        {
            _followTransform.parent = followTransform;
        }
        else
        {
            _followTransform.parent = originalParent;
        }
    }

    void LateUpdate()
    {
        UpdateInventoryPosition();

    }

    private void UpdateInventoryPosition()
    {
        if (cameraTransform == null && GameObject.FindGameObjectWithTag("MainCamera") != null)
        {
            cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
            _followTransform.position = InFrontOfCamera();
            _followTransform.localEulerAngles = Vector3.zero;
        }

        if (cameraTransform == null)
        {
            return;
        }

        // offset the character's body if available
        Vector3 worldOffset = Vector3.zero;

        if (followTransform)
        {
            worldOffset = followTransform.position - followTransform.TransformVector(offset);
        }

        Vector3 moveToPosition = new Vector3(worldOffset.x, cameraTransform.position.y - offset.y, worldOffset.z);

        Vector3 movePosition = InFrontOfCamera();
        transform.position = Vector3.SmoothDamp(transform.position, moveToPosition, ref velocity, movementSmoothing);

        transform.rotation = Quaternion.Lerp(transform.rotation, _followTransform.rotation, Time.deltaTime * rotateSpeed);

    }

    public  Vector3 InFrontOfCamera()
    {
        Vector3 position = Camera.main.transform.position + (Camera.main.transform.forward * 0.5f);
        return position;
    }
}
