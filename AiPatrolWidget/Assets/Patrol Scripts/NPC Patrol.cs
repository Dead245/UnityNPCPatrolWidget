using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

public class NPCPatrol : MonoBehaviour
{
    [SerializeField]
    GameObject npc;
    float instantiationYOffset = 1f;
    GameObject spawnedNPC;

    [SerializeField]
    SplineContainer splinePath;

    float walkSpeed = 5f;
    float stepLength = 0.05f;
    float splineLength;

    float distancePercent = 0;
    Vector3 currentPos, direction;
    Vector3 firstPos, firstDir;

    void Start()
    {
        firstPos = splinePath.EvaluatePosition(0);
        firstPos.y += instantiationYOffset;
        firstDir = getDirectionVector(0f, 0.01f);
        spawnedNPC = Instantiate(npc, firstPos, Quaternion.LookRotation(firstDir));
        splineLength = splinePath.CalculateLength();
    }

    void Update()
    {
        distancePercent += walkSpeed * Time.deltaTime / splineLength;

        if (distancePercent > 1.0f) { return; }

        Vector3 newPos = splinePath.EvaluatePosition(distancePercent);
        newPos.y += instantiationYOffset;
        spawnedNPC.transform.position = newPos;

        direction = getDirectionVector(distancePercent, distancePercent + stepLength);
        Quaternion rotation = Quaternion.LookRotation(direction);
        rotation.x = spawnedNPC.transform.rotation.x;
        rotation.z = spawnedNPC.transform.rotation.z;
        spawnedNPC.transform.rotation = rotation;

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
}
