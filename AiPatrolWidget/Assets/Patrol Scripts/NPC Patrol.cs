using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

public class NPCPatrol : MonoBehaviour
{
    [SerializeField]
    GameObject[] npcList;
    float instantiationYOffset = 1f;
    List<GameObject> spawnedNPCs = new List<GameObject>();

    [SerializeField]
    SplineContainer splinePath;

    float stepLength = 0.05f;
    float spawnTimer = 5f;
    int maxNPCs = 3;
    float splineLength;

    float timeSinceLastSpawn = 0;
    int currentSpawnIndex = 0;
    int npcListLength;

    float distancePercent = 0;
    Vector3 direction;
    Vector3 firstPos, firstDir;

    BezierKnot[] knots;

    private struct NPCInfo {
        GameObject npcObj;
        float distancePercent;
        float walkSpeed;
        float verticalSpawnOffset;
    }

    void Start()
    {
        firstPos = splinePath.EvaluatePosition(0);
        firstPos.y += instantiationYOffset;
        firstDir = getDirectionVector(0f, 0.01f);

        splineLength = splinePath.CalculateLength();
        npcListLength = npcList.Length;

        knots = splinePath.Spline.ToArray();
    }

    void Update()
    {
        NpcSpawning();
        foreach (var npc in spawnedNPCs) {
            //NPC Movement
            Vector3 newPos = splinePath.EvaluatePosition(distancePercent);
            newPos.y += instantiationYOffset;
            npc.transform.position = newPos;

            direction = getDirectionVector(distancePercent, distancePercent + stepLength);
            Quaternion rotation = Quaternion.LookRotation(direction);
            rotation.x = npc.transform.rotation.x;
            rotation.z = npc.transform.rotation.z;
            npc.transform.rotation = rotation;

            //TODO: Rewrite knot detection and movement so that it is seperate per npc.
            //Also rewrite knot detection to be more consistent.

                //Knot Detection
           // float closestKnot = splinePath.Spline.ConvertIndexUnit<Spline>(distancePercent, PathIndexUnit.Knot);
           // closestKnot = Mathf.RoundToInt(closestKnot); //Index of the closest knot on the spline

           // Vector3 localNPCPosition = splinePath.transform.InverseTransformPoint(npc.transform.position);
           // localNPCPosition.y -= instantiationYOffset;

                //Activates if NPC is within 'knotRadius' range of the knot, using local spaces
           // if (Vector3.Distance(knots[(int)closestKnot].Position, localNPCPosition) <= knotRadius)
           // {
           //     if ((int)closestKnot != lastActiveKnot)
           //     {
           //         activateKnotInteraction((int)closestKnot);
           //     }
           //     lastActiveKnot = (int)closestKnot;
           // }

            //Increase distance the NPC is along the spline (Divided by 2 to even out calculations along spline ends)
            //distancePercent += (walkSpeed * Time.deltaTime / splineLength) / 2;
        }
        

        

        
    }
    Vector3 getDirectionVector(float firstPercent, float secondPercent)
    {
        Vector3 direction;
        Vector3 _firstPos, _secondPos;

        _firstPos = splinePath.EvaluatePosition(firstPercent);
        _secondPos = splinePath.EvaluatePosition(secondPercent);

        direction = _secondPos - _firstPos;

        return direction;
    }

    void NpcSpawning()
    {
        if (timeSinceLastSpawn == 0 || timeSinceLastSpawn >= spawnTimer) {
            Instantiate(npcList[currentSpawnIndex], firstPos, Quaternion.LookRotation(firstDir));
            timeSinceLastSpawn = 0;
            Debug.Log("Spawned NPC.");
            if (currentSpawnIndex < npcListLength - 1) {
                currentSpawnIndex++;
            } else {
                currentSpawnIndex = 0;
            }
        }
        timeSinceLastSpawn += Time.deltaTime;
    }

    void activateKnotInteraction(int knotIndex) {
        Debug.Log("On Knot#: " + knotIndex);
    }

}
