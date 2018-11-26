using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : MonoBehaviour{
    public int width;
    public int height;

    private void Start()
    {
        
    }

    public int Count()
    {
        return width * height;
    }

    public void Init(int width, int height)
    {
        this.width = width;
        this.height = height;

    }
}
