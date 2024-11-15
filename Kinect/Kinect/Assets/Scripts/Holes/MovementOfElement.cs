using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holes
{
    public enum Block_States
    {
        noMovement,
        movingDown,
        movingUp,
        stable
    }

    public class MovementOfElement : MonoBehaviour
    {
        Hole holeScript;
        Block_States block_state;

        Transform _spawnPoint;
        public float goingUpSpeed, goingDownSpeed;
        public float upperHeightLimit, bottomHeightLimit;
        public float timeInStablePosition;
        public Vector3 rotation;
        public Vector3 offset;
        public float tempTime = 0f;

        AdjustDifficulty adjust;
        HolesManager holesManager;
        public Block_States Block_States => block_state;
        int firstScore;
        int endScore;

        private void FixedUpdate()
        {
        
            switch (block_state)
            {
                case Block_States.stable:

                    tempTime += Time.deltaTime;
                    if(tempTime >= timeInStablePosition)
                    {
                        MoveDownBlock();
                    }

                    break;

                case Block_States.movingUp:

                    transform.Translate(Vector3.up * goingUpSpeed * Time.deltaTime, Space.Self);

                    if (transform.position.y >= _spawnPoint.position.y + upperHeightLimit)
                    {
                        block_state = Block_States.stable;
                    }
                    firstScore = adjust.Score;

                    break;

                case Block_States.movingDown:

                    transform.Translate(Vector3.down * goingDownSpeed * Time.deltaTime, Space.Self);

                    if (transform.position.y <= _spawnPoint.position.y - bottomHeightLimit)
                    {
                        Destroy(gameObject);
                        holeScript.ChangeHoleState(false);

                    }
                    if (this.CompareTag(holesManager.selectedTag) && firstScore == adjust.Score)
                    {
                        adjust.restartBonus = true;
                    }
                    break;

                default:
                    Debug.LogWarning("No movement");

                    break;
            }
        }
        private void Start()
        {
            holeScript = transform.parent.gameObject.GetComponent<Hole>();
            _spawnPoint = holeScript.spawnPoint;
            adjust = GameObject.FindGameObjectWithTag("AdjDif").GetComponent<AdjustDifficulty>();
            timeInStablePosition = adjust.TimeInStablePosition();
            MoveUpBlock();
            holesManager = GameObject.FindGameObjectWithTag("hManager").GetComponent<HolesManager>();
        }
        private void Update()
        {
            adjust.restartBonus = false;

        }

        public void MoveUpBlock()
        {
            block_state = Block_States.movingUp;
        }

        public void MoveDownBlock()
        {
            block_state = Block_States.movingDown;
        }

        public void StabilizeBlock()
        {
            block_state = Block_States.movingDown;
        }


    }
}