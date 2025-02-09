using System;
using Interact;
using UnityEngine;

namespace UI
{
    public class UnitSelectionManagerUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _selectionAreaRectTransform;
        [SerializeField] private Canvas _canvas;

        private void Start()
        {
            InteractWorld.Instance.OnSelectionAreaStart += InteractWorld_OnSelectionAreaStart;
            InteractWorld.Instance.OnSelectionAreaEnd += InteractWorld_OnSelectionAreaEnd;

            this._selectionAreaRectTransform.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (this._selectionAreaRectTransform.gameObject.activeSelf)
            {
                UpdateVisual();
            }
        }

        private void OnDestroy()
        {
            InteractWorld.Instance.OnSelectionAreaStart -= InteractWorld_OnSelectionAreaStart;
            InteractWorld.Instance.OnSelectionAreaEnd -= InteractWorld_OnSelectionAreaEnd;

            this._selectionAreaRectTransform.gameObject.SetActive(false);
        }

        private void InteractWorld_OnSelectionAreaStart(object sender, EventArgs e)
        {
            this._selectionAreaRectTransform.gameObject.SetActive(true);

            this.UpdateVisual();
        }

        private void InteractWorld_OnSelectionAreaEnd(object sender, EventArgs e)
        {
            this._selectionAreaRectTransform.gameObject.SetActive(false);
        }

        private void UpdateVisual()
        {
            Rect selectionAreaRect = InteractWorld.Instance.GetSelectionAreaRect();
            float canvasScale = this._canvas.transform.localScale.x;

            this._selectionAreaRectTransform.anchoredPosition =
                new Vector2(selectionAreaRect.x, selectionAreaRect.y) / canvasScale;
            this._selectionAreaRectTransform.sizeDelta =
                new Vector2(selectionAreaRect.width, selectionAreaRect.height) / canvasScale;
        }
    }
}