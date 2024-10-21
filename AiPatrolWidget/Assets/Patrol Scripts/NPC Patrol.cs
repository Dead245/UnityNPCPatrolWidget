using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Splines;
using UnityEditor.TerrainTools;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PatrolScripts
{
    [Serializable]
    public class NPCPatrol : MonoBehaviour
    {
        [SerializeField]
        int maxNpcs = 5;

        NPCInfo[] npcList;
        int npcListLength;

        SplineContainer splinePath = null;
        float splineLength;

        List<NPCInfo> spawnedNPCs = new List<NPCInfo>();
        float spawnTimer = 5f;
        float timeSinceLastSpawn = 0;
        int currentSpawnIndex = 0;

        Vector3 direction;
        Vector3 firstPos, firstDir;

        BezierKnot[] knots;
        List<float> knotPercents = new List<float>();

        [System.Serializable]
        class NPCInfo
        {
            [Tooltip("Prefab of the NPC")]
            public GameObject npcObj = null;
            [Tooltip("How far down the spline the NPC will spawn as a decimal percentage")]
            public float distancePercent = 0;
            [Tooltip("How fast the NPC moves through the spline")]
            public float walkSpeed = 5f;
            [Tooltip("Vertical Offset for when the NPC spawns, 0 means they spawn centered on the spline")]
            public float verticalSpawnOffset = 1f;
            [Tooltip("The most recent knot on the spline the NPC has passed")]
            public int currentKnotProgress = 0;
        }

        void Start()
        {
            firstPos = splinePath.EvaluatePosition(0);

            //Gets beginning direction of spline by checking the direction between 0% and 1%
            firstDir = getDirectionVector(0f, 0.01f);

            splineLength = splinePath.CalculateLength();
            npcListLength = npcList.Length;

            knots = splinePath.Spline.ToArray();

            //Calculate the % distance each knot is on the spline
            for (int i = 0; i < knots.Length; i++)
            {
                float knotDistance = splinePath.Spline.ConvertIndexUnit<Spline>(i, PathIndexUnit.Knot, PathIndexUnit.Distance);
                knotPercents.Add(knotDistance / splineLength);
            }
        }

        void Update()
        {
            NpcSpawning();
            foreach (var npc in spawnedNPCs)
            {
                //NPC Movement
                Vector3 newPos = splinePath.EvaluatePosition(npc.distancePercent);
                newPos.y += npc.verticalSpawnOffset;
                npc.npcObj.transform.position = newPos;

                float newStepLength = npc.walkSpeed * Time.deltaTime / splineLength;

                direction = getDirectionVector(npc.distancePercent, npc.distancePercent + newStepLength);

                //direction should only be zero if at the end of the spline.
                if (direction != Vector3.zero)
                {
                    Quaternion rotation = Quaternion.LookRotation(direction);
                    rotation.x = npc.npcObj.transform.rotation.x;
                    rotation.z = npc.npcObj.transform.rotation.z;
                    npc.npcObj.transform.rotation = rotation;
                }

                //Reset knotProgress and distancePercent once over 1 full loop around the closed spline.
                if (splinePath.Spline.Closed && npc.distancePercent >= 1)
                {
                    npc.distancePercent -= 1;
                    npc.currentKnotProgress = 0;
                }

                //Increment the NPC's distancePercent by stepLength
                if (npc.distancePercent < 1 || splinePath.Spline.Closed)
                {
                    npc.distancePercent += newStepLength;
                }

                //Knot Progression Detection
                if (npc.currentKnotProgress < splinePath.Spline.Count - 1)
                {
                    if (npc.distancePercent >= knotPercents[npc.currentKnotProgress + 1])
                    {
                        npc.currentKnotProgress += 1;
                    }
                }
            }

        }
        Vector3 getDirectionVector(float firstPercent, float secondPercent)
        {
            Vector3 direction;
            Vector3 _firstPos, _secondPos;

            _firstPos = splinePath.EvaluatePosition(firstPercent);

            if (secondPercent > 1 && splinePath.Spline.Closed)
            {
                secondPercent = 1 - secondPercent;
            }

            _secondPos = splinePath.EvaluatePosition(secondPercent);

            direction = _secondPos - _firstPos;

            return direction;
        }

        void NpcSpawning()
        {
            if ((timeSinceLastSpawn == 0 || timeSinceLastSpawn >= spawnTimer) && spawnedNPCs.Count < maxNpcs)
            {
                Vector3 newOffset = firstPos;
                newOffset.y += npcList[currentSpawnIndex].verticalSpawnOffset;

                GameObject newNPC = Instantiate(npcList[currentSpawnIndex].npcObj, newOffset, Quaternion.LookRotation(firstDir), this.gameObject.transform);

                NPCInfo newInfo = new NPCInfo()
                {
                    distancePercent = 0,
                    npcObj = newNPC,
                    verticalSpawnOffset = npcList[currentSpawnIndex].verticalSpawnOffset,
                    walkSpeed = npcList[currentSpawnIndex].walkSpeed
                };

                spawnedNPCs.Add(newInfo);

                timeSinceLastSpawn = 0;

                if (currentSpawnIndex < npcListLength - 1)
                {
                    currentSpawnIndex++;
                }
                else
                {
                    currentSpawnIndex = 0;
                }
            }
            timeSinceLastSpawn += Time.deltaTime;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(NPCPatrol))]
    class NPCPatrolEditor: Editor {
        public override void OnInspectorGUI()
        {
            var npcPatrol = (NPCPatrol)target;
            if (npcPatrol == null) return;

            base.OnInspectorGUI();
        }

    }
#endif
}
