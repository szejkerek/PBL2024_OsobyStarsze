using UnityEngine;

public class HandsDataTracker : MonoBehaviour
{
    [SerializeField] GameObject handPrefab;

    private Vector3 handPosition;
    private Quaternion handRotation;
    private Vector3 lastHandPosition = Vector3.zero;
    private float lastSpeedUpdateTime;
    private float lastFrameTime;
    
    public HandBehaviourHandler handBehaviourHandler;

    public FrameHandData GetHandFrameData()
    {
        if (lastHandPosition == Vector3.zero)
        {
            lastHandPosition = handPrefab.transform.position;
            lastFrameTime = Time.time;

            return null;
        }

        FrameHandData frameData = new FrameHandData();
        
        Vector3 currentPosition = handPrefab.transform.position;
        Quaternion currentRotation = handPrefab.transform.rotation;
        
        float deltaTime = Time.time - lastFrameTime;

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
        
        frameData.position = currentPosition;
        frameData.rotationEuler = currentRotation.eulerAngles;
        frameData.rotation = currentRotation;
        
        lastHandPosition = currentPosition;
        lastFrameTime = Time.time;

        return frameData;
    }

}
