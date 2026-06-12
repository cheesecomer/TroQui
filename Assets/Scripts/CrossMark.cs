using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(CanvasRenderer))]
public class CrossMark : MaskableGraphic
{
    [SerializeField] private float thickness = 40f;
    [SerializeField] private int segments = 12;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        var rect = rectTransform.rect;
        float size = Mathf.Min(rect.width, rect.height);
        float length = size * 0.75f;

        AddRoundedBar(vh, new Vector2(-length / 2f, -length / 2f), new Vector2(length / 2f, length / 2f));
        AddRoundedBar(vh, new Vector2(-length / 2f, length / 2f), new Vector2(length / 2f, -length / 2f));
    }

    private void AddRoundedBar(VertexHelper vh, Vector2 start, Vector2 end)
    {
        Vector2 dir = (end - start).normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x);

        float half = thickness / 2f;

        Vector2 p0 = start + normal * half;
        Vector2 p1 = end + normal * half;
        Vector2 p2 = end - normal * half;
        Vector2 p3 = start - normal * half;

        int idx = vh.currentVertCount;

        vh.AddVert(p0, color, Vector2.zero);
        vh.AddVert(p1, color, Vector2.zero);
        vh.AddVert(p2, color, Vector2.zero);
        vh.AddVert(p3, color, Vector2.zero);

        vh.AddTriangle(idx, idx + 1, idx + 2);
        vh.AddTriangle(idx, idx + 2, idx + 3);

        AddCircleCap(vh, start, half);
        AddCircleCap(vh, end, half);
    }

    private void AddCircleCap(VertexHelper vh, Vector2 center, float radius)
    {
        int centerIndex = vh.currentVertCount;
        vh.AddVert(center, color, Vector2.zero);

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.PI * 2f * i / segments;
            Vector2 p = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            vh.AddVert(p, color, Vector2.zero);

            if (i > 0)
            {
                vh.AddTriangle(centerIndex, centerIndex + i, centerIndex + i + 1);
            }
        }
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        SetVerticesDirty();
    }
}