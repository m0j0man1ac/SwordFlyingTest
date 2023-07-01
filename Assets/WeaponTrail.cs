using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrail : MonoBehaviour
{
    [SerializeField] public Transform tip, baseT;
    [SerializeField] GameObject meshObj;

    [SerializeField] int trailFrameLifetime;

    Mesh trailMesh;
    Vector3[] vertices;
    int[] triangles;
    int frameCount;

    int vertCount = 2;
    int triCount;

    Vector3 prevTip, prevBase;

    const int NUM_VERTICES = 12;

    // Start is called before the first frame update
    void Start()
    {
        trailMesh = new Mesh();
        meshObj.GetComponent<MeshFilter>().mesh = trailMesh;

        vertices = new Vector3[trailFrameLifetime * 2];
        triangles = new int[trailFrameLifetime * NUM_VERTICES - NUM_VERTICES];

        prevBase = baseT.position;
        prevTip = tip.position;

        int i = 0;
        while (i < vertices.Length)
        {
            if (i % 2 == 0) vertices[i++] = baseT.position;
            else vertices[i++] = tip.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(frameCount == trailFrameLifetime * NUM_VERTICES)
            frameCount = 0;

        if (vertCount == trailFrameLifetime * 2) vertCount = 0;
        if (triCount == trailFrameLifetime * NUM_VERTICES) triCount = 0;

        vertices[vertCount] = baseT.position;
        vertices[vertCount+1] = tip.position;

        for (int i=0; i<=2; i++)
        {
            triangles[triCount+i] = (vertices.Length + vertCount+i-2) % vertices.Length;
            triangles[triCount+3+i] = (vertices.Length + vertCount-i) % vertices.Length;

            triangles[triCount + 6 + i] = (vertices.Length + vertCount + i - 1) % vertices.Length;
            triangles[triCount + 9 + i] = (vertices.Length + vertCount - i + 1) % vertices.Length;
        }

        //assign verts and tris
        trailMesh.vertices = vertices;
        trailMesh.triangles = triangles;

        //counter update for next frame 
        vertCount += 2;
        vertCount %= vertices.Length;

        triCount += 12;
        triCount %= triangles.Length;

        frameCount += NUM_VERTICES;

        //meshObj.GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
        trailMesh.RecalculateBounds();

        //save current pos as previous
        prevBase = baseT.position;
        prevTip = tip.position;
    }
}
