using UnityEngine;

public class HandsDataTracker : MonoBehaviour
{
    [SerializeField] GameObject handPrefab;

    private Vector3 handPosition;
    private Quaternion handRotation;
    private Vector3 lastHandPosition;
    private float lastSpeedUpdateTime;
    private float lastFrameTime;

    public FrameHandData GetHandFrameData()
    {
        FrameHandData frameData = new FrameHandData();

        handPosition = handPrefab.transform.position;
        handRotation = handPrefab.transform.rotation;

        frameData.position = handPosition;
        frameData.rotationEuler = handRotation.eulerAngles;

        frameData.direction = handPosition - lastHandPosition;

        float deltaTime = Time.time - lastFrameTime;
        frameData.speed = frameData.direction.magnitude / deltaTime;

        lastHandPosition = handPosition;
        lastFrameTime = Time.time;

        return frameData;
    }
}
