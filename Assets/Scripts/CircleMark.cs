using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(CanvasRenderer))]
public class CircleMark : MaskableGraphic
{
    [SerializeField] private float thickness = 24f;
    [SerializeField] private int segments = 64;

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect rect = rectTransform.rect;
        float radius = Mathf.Min(rect.width, rect.height) * 0.5f;
        float outer = radius;
        float inner = Mathf.Max(0f, radius - thickness);

        for (var i = 0; i < segments; i++)
        {
            float a0 = Mathf.PI * 2f * i / segments;
            float a1 = Mathf.PI * 2f * (i + 1) / segments;

            var o0 = new Vector3(Mathf.Cos(a0) * outer, Mathf.Sin(a0) * outer);
            var o1 = new Vector3(Mathf.Cos(a1) * outer, Mathf.Sin(a1) * outer);
            var i0 = new Vector3(Mathf.Cos(a0) * inner, Mathf.Sin(a0) * inner);
            var i1 = new Vector3(Mathf.Cos(a1) * inner, Mathf.Sin(a1) * inner);

            int idx = vh.currentVertCount;

            vh.AddVert(o0, color, Vector2.zero);
            vh.AddVert(o1, color, Vector2.zero);
            vh.AddVert(i1, color, Vector2.zero);
            vh.AddVert(i0, color, Vector2.zero);

            vh.AddTriangle(idx, idx + 1, idx + 2);
            vh.AddTriangle(idx, idx + 2, idx + 3);
        }
    }
}