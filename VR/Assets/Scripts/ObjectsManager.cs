using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Difficulty", menuName = "Difficulty", order = 0)]
class Difficulty : ScriptableObject
{
    public float duration;
    public float amount;
}

public class ObjectsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Difficulty difficulty;
    private GameObject prefab;
    public static ObjectsManager instance;

    [SerializeField] private float[] xValues = { 1.5f, 0.96f, 0.45f, -0.06f };
    [SerializeField] private float[] yValues = { 2.68f, 2.25f, 1.85f, 1.425f };
    private float currentX;
    private float currentY;
    private float currentZ = 16.3f;
    private float spawnTime;
    private float despawnTime;
    public float reactionTime;
    private Vector3 targetPosition;

    private List<GameObject> currentObjects = new List<GameObject>();
    private Coroutine currentCoroutine;

    public float timeToClose;
    void Start()
    {
        timeToClose = difficulty.duration;

        if (instance == null)
            instance = this;

        for (int i = 0; i < difficulty.amount; i++)
        {
            CreateObject();
        }
    }

    private void CreateObject()
    {
        prefab = prefabs[Random.Range(0, prefabs.Length)];
        GameObject obj = Instantiate(prefab, transform.localPosition, Quaternion.identity, transform);
        currentObjects.Add(obj);

        GenerateObjectPosition(obj);

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(SpawnAndDespawn(obj));
    }

    private void GenerateObjectPosition(GameObject obj)
    {
        currentX = xValues[Random.Range(0, xValues.Length)];
        currentY = yValues[Random.Range(0, yValues.Length)];
        targetPosition = new Vector3(currentX, currentY, currentZ);
        obj.transform.localPosition = targetPosition;
    }

    private IEnumerator SpawnAndDespawn(GameObject obj)
    {
        spawnTime = Time.time;

        yield return new WaitForSeconds(difficulty.duration - 1f);

        CheckDespawnTime();
    }

    public void CheckDespawnTime()
    {
        despawnTime = Time.time;
        reactionTime = despawnTime - spawnTime;
        Debug.Log(reactionTime);

        if (currentObjects.Count > 0)
        {
            Destroy(currentObjects[0]);
            currentObjects.RemoveAt(0);
        }

        CreateObject();
    }
}
