using UnityEngine;

namespace PaleozoicBeach.Runtime
{
    // Fit the existing reference UI inside the physical safe area, including notches.
    public sealed class PhoneLayout : MonoBehaviour
    {
        public RectTransform content;
        private Rect lastSafeArea;
        private Vector2 lastCanvasSize;
        private void LateUpdate()
        {
            if (content == null || Screen.width == 0 || Screen.height == 0) return;
            var canvas = (RectTransform)content.parent;
            Rect safe = Screen.safeArea;
            Vector2 size = canvas.rect.size;
            if (safe == lastSafeArea && size == lastCanvasSize) return;
            lastSafeArea = safe; lastCanvasSize = size;
            Vector2 center = safe.center;
            content.anchorMin = content.anchorMax = new Vector2(center.x / Screen.width, center.y / Screen.height);
            content.pivot = new Vector2(.5f, .5f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = new Vector2(1220, 680);
            float width = size.x * safe.width / Screen.width - 24;
            float height = size.y * safe.height / Screen.height - 24;
            content.localScale = Vector3.one * Mathf.Max(.01f, Mathf.Min(width / 1220, height / 680));
        }
    }
}
