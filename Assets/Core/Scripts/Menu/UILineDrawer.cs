using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class UILineDrawer : MaskableGraphic
{
    public Vector2 P1;
    public Vector2 P2;

    public float LineWidth = 50;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        UIVertex[] quad = new UIVertex[4];

        var hyp = Vector2.Distance(P1, P2);

        var cos = (P1.x - P2.x) / hyp;
        var sin = (P1.y - P2.y) / hyp;

        var dx = LineWidth / 2 * sin;
        var dy = LineWidth / 2 * cos;

        quad[0].position = new Vector3(P1.x - dx, P1.y + dy, 0);
        quad[1].position = new Vector3(P1.x + dx, P1.y - dy, 0);
        quad[2].position = new Vector3(P2.x - dx, P2.y + dy, 0);
        quad[3].position = new Vector3(P2.x + dx, P2.y - dy, 0);

        quad[0].color = color;
        quad[1].color = color;
        quad[2].color = color;
        quad[3].color = color;

        quad[0].uv0 = new Vector2(0, 0);
        quad[1].uv0 = new Vector2(1, 0);
        quad[2].uv0 = new Vector2(1, 1);
        quad[3].uv0 = new Vector2(0, 1);

        for (int i = 0; i < 4; i++)
            vh.AddVert(quad[i]);
        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(1, 2, 3);
    }
}