using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawAttackLine : MonoBehaviour
{
    public int numberOfVertices;
    public float radius;
    public float startWidth;
    public float endWidth;

    private LineRenderer lineRenderer;


    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
    }

    private void Update()
    {
        DrawCircle();
    }

    public void DrawCircle()
    {
        lineRenderer.positionCount = numberOfVertices + 1; // mais um para o ultimo ser igual ao primeir, se n fica com uma fenda

        float x;
        float y;
        float z = 0;

        float angle = 0f;


        for (int i = 0; i < (numberOfVertices + 1); i++)
        {
            x = (float)(Math.Sin((Math.PI / 180) * angle) * radius);
            y = (float)(Math.Cos((Math.PI / 180) * angle) * radius);

            lineRenderer.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / numberOfVertices);
        }
    }

}

