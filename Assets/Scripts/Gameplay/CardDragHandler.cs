using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    private Transform startParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        startParent = transform.parent;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = new Vector2(eventData.delta.x, eventData.delta.y);
        rectTransform.anchoredPosition += delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        GameObject targetObject = eventData.pointerCurrentRaycast.gameObject;
        if (targetObject != null && targetObject.TryGetComponent(out PlayFieldSlot slot))
        {
            Card card = GetComponent<Card>();
            if (slot.CanPlace(card))
                slot.PlaceCard(card);
            else
                transform.position = startPosition;
        }
        else
        {
            transform.position = startPosition;
        }
    }
}
