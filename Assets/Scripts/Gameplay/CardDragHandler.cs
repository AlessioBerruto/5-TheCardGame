using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;
    private int originalSiblingIndex;

    private bool isDraggable = false;

    public void SetDraggable(bool canDrag)
    {
        isDraggable = canDrag;
    }
    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        originalPosition = rectTransform.position;
        originalRotation = rectTransform.rotation;
        originalParent = rectTransform.parent;
        originalSiblingIndex = rectTransform.GetSiblingIndex();

        rectTransform.SetParent(canvas.transform, true);
        canvasGroup.blocksRaycasts = false;
        rectTransform.rotation = Quaternion.identity;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        canvasGroup.blocksRaycasts = true;

        GameObject target = eventData.pointerEnter;
        if (target != null)
        {
            PlayFieldSlot slot = target.GetComponent<PlayFieldSlot>();
            if (slot != null)
            {
                if (slot.CanPlace(GetComponent<Card>()))
                {
                    rectTransform.SetParent(slot.transform, false);
                    slot.PlaceCard(GetComponent<Card>());
                    return;
                }
            }

            DiscardPile pile = target.GetComponent<DiscardPile>();
            if (pile != null)
            {
                pile.AddCard(GetComponent<Card>());
                rectTransform.SetParent(pile.transform, false);
                return;
            }
        }

        ReturnToOriginalPosition();
    }

    private void ReturnToOriginalPosition()
    {
        rectTransform.SetParent(originalParent, false);
        rectTransform.SetSiblingIndex(originalSiblingIndex);
        rectTransform.position = originalPosition;
        rectTransform.rotation = originalRotation;
    }
}
