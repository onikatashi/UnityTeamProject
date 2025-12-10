using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public GameObject linePrefab;               // 라인 UI(Image)
    public RectTransform lineParent;            // 라인 부모
    public float lineThickness = 6f;

    private readonly List<GameObject> activeLines = new List<GameObject>();


    public void DrawAllConnections(NodeButton[,] nodes, int maxFloor, int maxColumn)
    {
        ClearAllLines();

        for (int f = 0; f < maxFloor; f++)
        {
            for (int c = 0; c < maxColumn; c++)
            {
                NodeButton from = nodes[f, c];
                if (from == null || from.nextNodes == null) continue;

                foreach (NodeButton to in from.nextNodes)
                {
                    DrawLine(from, to);
                }
            }
        }
    }


    public void ClearAllLines()
    {
        foreach (var line in activeLines)
        {
            if (line != null) Destroy(line);
        }
        activeLines.Clear();
    }


    private void DrawLine(NodeButton from, NodeButton to)
    {
        if (from == null || to == null) return;

        GameObject lineObj = Instantiate(linePrefab, lineParent);
        activeLines.Add(lineObj);

        RectTransform rt = lineObj.GetComponent<RectTransform>();

        RectTransform fromRect = from.GetComponent<RectTransform>();
        RectTransform toRect = to.GetComponent<RectTransform>();

        // 1. UI anchoredPosition 직접 사용 (가장 정확한 방법)
        Vector2 start = fromRect.anchoredPosition;
        Vector2 end = toRect.anchoredPosition;

        // 2. 중간 지점 배치
        Vector2 center = (start + end) * 0.5f;
        rt.anchoredPosition = center;

        // 3. 라인 길이 설정
        float distance = Vector2.Distance(start, end);
        rt.sizeDelta = new Vector2(distance, lineThickness);

        // 4. 각도 적용
        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;
        rt.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
