using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exporter : MonoBehaviour
{
    public string name;

    private void Start()
    {
        exportGlb(name);
    }

    public void exportGlb(string gameObjectName)
    {
        ExportGlb.ExportGLBByName(gameObjectName);
    }
}
