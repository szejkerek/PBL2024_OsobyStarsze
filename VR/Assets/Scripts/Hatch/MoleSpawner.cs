using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MoleSpawner : MonoBehaviour
{
    [Header("Mole Configuration")]
    [SerializeField] private List<GameObject> molePrefabs;
    [SerializeField] private float spawnForce = 500f;
    [SerializeField] private ForceMode spawnForceMode = ForceMode.Impulse;

    [Header("Hatch Configuration")]
    [SerializeField] private List<HatchController> hatchControllers;
    [SerializeField] private float preSpawnDowntime = 1f;
    [SerializeField] private float spawnDowntime = 0.5f;
    [SerializeField] private float lifetimeDowntime = 3f;
    [SerializeField] private float eolDowntime = 1f;
    [SerializeField] private float spawnInterval = 2f;

    private List<int> availableHatches = new List<int>();
    private List<bool> hatchAvailability;

    private void Start()
    {
        InitializeHatches();
        StartSpawning();
    }

    private void InitializeHatches()
    {
        int hatchCount = hatchControllers.Count;
        hatchAvailability = Enumerable.Repeat(true, hatchCount).ToList();
        availableHatches.Clear();
        for (int i = 0; i < hatchCount; i++)
        {
            availableHatches.Add(i);
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnMolesRoutine());
    }

    private IEnumerator SpawnMolesRoutine()
    {
        while (true)
        {
            if (availableHatches.Count > 0)
            {
                // Randomly select an available hatch
                int randomIndex = Random.Range(0, availableHatches.Count);
                int hatchIndex = availableHatches[randomIndex];
                availableHatches.RemoveAt(randomIndex);

                if (hatchAvailability[hatchIndex])
                {
                    StartCoroutine(HandleHatch(hatchIndex));
                    yield return new WaitForSeconds(spawnInterval);
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator HandleHatch(int hatchIndex)
    {
        hatchAvailability[hatchIndex] = false;

        HatchController hatchController = hatchControllers[hatchIndex];

        // Open Hatch
        hatchController.ReleaseAllDoors();
        yield return new WaitForSeconds(preSpawnDowntime);

        // Spawn Mole
        SpawnMole(hatchController.spawnPoint.transform);
        yield return new WaitForSeconds(spawnDowntime);

        // Close Hatch and store the coroutine reference
        Coroutine keepClosedCoroutine = StartCoroutine(KeepHatchClosed(hatchController));
        yield return new WaitForSeconds(lifetimeDowntime);

        // Stop the specific KeepHatchClosed coroutine
        StopCoroutine(keepClosedCoroutine);

        // Reopen Hatch
        hatchController.ReleaseAllDoors();
        yield return new WaitForSeconds(eolDowntime);

        // Make Hatch Available
        hatchAvailability[hatchIndex] = true;
        availableHatches.Add(hatchIndex); // Add back to available hatches
    }

    private IEnumerator KeepHatchClosed(HatchController hatchController)
    {
        while (true)
        {
            hatchController.ApplyDoorForces();
            yield return null;
        }
    }

    private void SpawnMole(Transform spawnPoint)
    {
        if (molePrefabs.Count == 0) return;

        GameObject molePrefab = molePrefabs[Random.Range(0, molePrefabs.Count)];
        GameObject spawnedMole = Instantiate(molePrefab, spawnPoint.position, Quaternion.identity);

        if (spawnedMole.TryGetComponent(out Rigidbody rb))
        {
            Vector3 upwardForce = spawnPoint.up * spawnForce;
            rb.AddForce(upwardForce, spawnForceMode);
        }

        Destroy(spawnedMole, lifetimeDowntime + eolDowntime);
    }
}