using UnityEngine;

// Uniformly scales this rect to fit its parent, counting children that hang outside it (e.g. tabs on a book).
// Unlike an AspectRatioFitter, contents scale along with it instead of spilling out on narrow screens.
// Needs centred anchors; the rect's own size is the design size.
public class ScaleToFit : MonoBehaviour
{
    private Vector2 fittedTo;

    private void LateUpdate()
    {
        // parent changes size on rotation / fold / safe area changes
        if (((RectTransform)transform.parent).rect.size != fittedTo) Fit();
    }

    [ContextMenu("Fit")]
    public void Fit()
    {
        var self = (RectTransform)transform;
        fittedTo = ((RectTransform)self.parent).rect.size;

        // bounds of this rect plus its direct children, in unscaled local space
        Vector2 min = self.rect.min, max = self.rect.max;
        var corners = new Vector3[4];
        foreach (RectTransform child in self)
        {
            if (!child.gameObject.activeSelf) continue;
            child.GetWorldCorners(corners);
            foreach (var corner in corners)
            {
                Vector2 p = self.InverseTransformPoint(corner);
                min = Vector2.Min(min, p);
                max = Vector2.Max(max, p);
            }
        }

        Vector2 size = max - min;
        float scale = Mathf.Min(fittedTo.x / size.x, fittedTo.y / size.y);
        self.localScale = new Vector3(scale, scale, 1);
        self.anchoredPosition = -(min + max) / 2 * scale; // centre the whole thing, overhang included
    }
}
