using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GradientHorizontal : BaseMeshEffect {
    [SerializeField]
    public Color leftColor;
    [SerializeField]
    public Color rightColor;

    public void Change(ref List<Vector2> list)
    {
        Vector2 v = list[0];
        v.x = 200;
        list[0] = v;
        list.Add(new Vector2(1, 1));
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }
        var list = new List<UIVertex>();
        vh.GetUIVertexStream(list);
        var count = list.Count;
        float left = 0;
        float right = 0;
        for (int i = 0; i < count; i++)
        {
            var vertex = list[i].position.x;
            Debug.Log(vertex);
            if (vertex > right)
            {
                right = vertex;
            }
            else if (vertex < left)
            {
                left = vertex;
            }
        }
        var weight = right - left;
        for (int i = 0; i < list.Count; i++)
        {
            var vertex = list[i];
            var liner = (vertex.position.x - left) / weight;

            Color color = Color.Lerp(leftColor, rightColor, liner);
            vertex.color = color;
            list[i] = vertex;
        }
        vh.Clear();
        vh.AddUIVertexTriangleStream(list);
    }
}
