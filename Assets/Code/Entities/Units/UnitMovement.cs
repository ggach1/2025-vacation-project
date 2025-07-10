using Code.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Entities.Units
{
    public class UnitMovement : MonoBehaviour
    {
        public float moveSpeed = 3f;

        public void MoveAlong(List<HexTile> path)
        {
            StopAllCoroutines();
            StartCoroutine(MovePath(path));
        }

        private IEnumerator MovePath(List<HexTile> path)
        {
            foreach (var tile in path)
            {
                Vector3 target = tile.transform.position;
                while (Vector3.Distance(transform.position, target) > 0.05f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                    yield return null;
                }
            }
        }
    }
}

