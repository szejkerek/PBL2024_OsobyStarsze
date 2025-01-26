using UnityEngine;

public class HandsDataTracker : MonoBehaviour
{
    [SerializeField] GameObject handPrefab;

    private Vector3 handPosition;
    private Quaternion handRotation;
    private Vector3 lastHandPosition = Vector3.zero;
    private float lastSpeedUpdateTime;
    private float lastFrameTime;

    public FrameHandData GetHandFrameData()
    {
        // If this is the first frame, initialize the cached values and skip calculations
        if (lastHandPosition == Vector3.zero)
        {
            lastHandPosition = handPrefab.transform.position;
            lastFrameTime = Time.time;

            return null; // Skip the first frame by returning null or an indicator
        }

        FrameHandData frameData = new FrameHandData();

        // Retrieve the current hand position and rotation
        Vector3 currentPosition = handPrefab.transform.position;
        Quaternion currentRotation = handPrefab.transform.rotation;

        // Calculate delta time since the last frame
        float deltaTime = Time.time - lastFrameTime;

        // Compute direction and speed
        if (deltaTime > Mathf.Epsilon)
        {
            frameData.direction = currentPosition - lastHandPosition;
            frameData.speed = frameData.direction.magnitude / deltaTime;
        }
        else
        {
            frameData.direction = Vector3.zero;
            frameData.speed = 0f;
        }

        // Store position and rotation data
        frameData.position = currentPosition;
        frameData.rotationEuler = currentRotation.eulerAngles;
        frameData.rotation = currentRotation;

        // Cache current data for the next frame
        lastHandPosition = currentPosition;
        lastFrameTime = Time.time;

        return frameData;
    }

}
