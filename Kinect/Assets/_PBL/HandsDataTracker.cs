using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Plugins.KinectModule.Data;

public struct FrameHandData
{
    public Vector3 position;
    public Vector3 rotationEuler;
    public Vector3 skierowanie;
    public float speed;
    public bool isDetected;
}

public class HandsDataTracker : MonoBehaviour {

    [SerializeField]
    private BodyTracker bodyTracker;

    [SerializeField] private JointId handJointId;

    private Vector3 handPosition;
    private Quaternion handRotation;
    private bool isDetected;

    private Vector3 lastHandPosition;
    private float lastSpeedUpdateTime;
    private float lastFrameTime;

    private FrameHandData GetHandFrameData() {
        FrameHandData frameData = new FrameHandData();

        if (bodyTracker.CanGetUpdateData()) {
            (handPosition, handRotation) = bodyTracker.GetDataByJoint(handJointId, ref isDetected);

            frameData.position = handPosition;
            frameData.rotationEuler = handRotation.eulerAngles;
            frameData.isDetected = isDetected;
            
            frameData.skierowanie = handPosition - lastHandPosition;

            float deltaTime = Time.time - lastFrameTime;
            frameData.speed = frameData.skierowanie.magnitude / deltaTime;

            lastHandPosition = handPosition;
            lastFrameTime = Time.time;
        }

        Debug.Log($"{handJointId} hand Position: {frameData.position}, Speed: {frameData.speed}");

        return frameData;
    }
}