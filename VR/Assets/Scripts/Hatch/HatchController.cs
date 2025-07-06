using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HatchController : MonoBehaviour
{
    [Header("Force Configuration")]
    [SerializeField] private Transform forceTarget;
    [SerializeField] private float forceStrength = 100f;
    [SerializeField] private ForceMode forceType = ForceMode.Force;
    [SerializeField] private float rotationStabilizationForce = 50f;
    [SerializeField] private float releaseImpulseStrength = 50f;

    [Header("Door References")]
    [SerializeField] public List<Rigidbody> hatchDoors = new List<Rigidbody>();
    [SerializeField] public GameObject spawnPoint;

    private List<Quaternion> _initialRotations = new List<Quaternion>();
    private bool _shouldApplyForces; // New control flag

    private void Start()
    {
        StoreInitialRotations();
        ValidateForceTarget();
    }

    private void FixedUpdate() // Physics updates belong here
    {
        if (!_shouldApplyForces) return;

        foreach (int i in Enumerable.Range(0, hatchDoors.Count))
        {
            Rigidbody door = hatchDoors[i];
            if (door == null) continue;

            // Position force
            Vector3 positionForce = CalculatePositionForce(door.position);
            door.AddForce(positionForce, forceType);

            // Rotation stabilization
            Quaternion rotationForce = CalculateRotationForce(door.rotation, _initialRotations[i]);
            door.AddTorque(new Vector3(rotationForce.x, rotationForce.y, rotationForce.z) * rotationStabilizationForce);
        }
    }

    // Add these control methods
    public void StartApplyingForces() => _shouldApplyForces = true;
    public void StopApplyingForces() => _shouldApplyForces = false;


    private void StoreInitialRotations()
    {
        _initialRotations.Clear();
        foreach (Rigidbody door in hatchDoors)
        {
            _initialRotations.Add(door.rotation);
        }
    }

    private void ValidateForceTarget()
    {
        if (!forceTarget)
        {
            Debug.LogError("Force target not assigned!", gameObject);
            forceTarget = new GameObject("ForceTarget_Fallback").transform;
            forceTarget.position = Vector3.zero;
        }
    }

    private Vector3 CalculatePositionForce(Vector3 doorPosition)
    {
        Vector3 directionToTarget = (forceTarget.position - doorPosition).normalized;
        return directionToTarget * forceStrength;
    }

    private Quaternion CalculateRotationForce(Quaternion currentRotation, Quaternion targetRotation)
    {
        return targetRotation * Quaternion.Inverse(currentRotation);
    }

    public void ReleaseAllDoors()
    {
        foreach (Rigidbody door in hatchDoors)
        {
            if (door != null)
            {
                door.linearVelocity = Vector3.zero;
                door.angularVelocity = Vector3.zero;
                // Apply downward impulse force
                door.AddForce(Vector3.down * releaseImpulseStrength, ForceMode.Impulse);
            }
        }
    }
}