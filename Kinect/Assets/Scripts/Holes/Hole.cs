using Plugins.KinectModule.Scripts.Events;
using UnityEngine;

namespace Holes
{
    public class Hole : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        public Transform spawnPoint;

        [SerializeField]
        private GameplayController controller;

        #endregion

        #region Private Fields

        private GameObject spawnedObject;
        private TriggerKinectObject trigger;
        private HoleID id;
        #endregion

        #region Public Properties

        public bool IsOccupied { get; private set; }

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            id = GetComponent<HoleID>();
        }
        private void Awake()
        {
            trigger = GetComponent<TriggerKinectObject>();
            AssignCallback();
        }

        #endregion

        #region Public Methods

        public void SpawnEnemy(GameObject objectToSpawn)
        {
            if (!IsOccupied)
            {
                spawnedObject = Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation, transform);
                spawnedObject.transform.Rotate(spawnedObject.GetComponent<MovementOfElement>().rotation);
                spawnedObject.transform.position = new Vector3(spawnedObject.transform.position.x + spawnedObject.GetComponent<MovementOfElement>().offset.x,
                                                               spawnedObject.transform.position.y + spawnedObject.GetComponent<MovementOfElement>().offset.y,
                                                               spawnedObject.transform.position.z + spawnedObject.GetComponent<MovementOfElement>().offset.z);
                ChangeHoleState(true);
            }
            else
            {
                Debug.LogWarning("Couldn't spawn enemy under " + gameObject.name + " (is occupied)");
            }
        }

        public void ChangeHoleState(bool state)
        {
            if (state == IsOccupied)
            {
                Debug.LogWarning("Can't change to the same state");

                return;
            }

            IsOccupied = state;
        }

        public void HitElement()
        {
            if (spawnedObject != null) {
                MovementOfElement movement = spawnedObject.GetComponent<MovementOfElement>();
                if (movement != null && movement.Block_States != Block_States.movingDown) {
                    movement.MoveDownBlock();
                    controller.CheckSpawnedObject(spawnedObject, id.id);
                }
            }
        }

        #endregion

        #region Private Methods

        private void AssignCallback()
        {
            trigger.onTrigger.AddListener(HitElement);
        }

        #endregion
    }
}