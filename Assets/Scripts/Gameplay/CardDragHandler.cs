using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Canvas canvas;
    Vector3 originalPosition;
    Quaternion originalRotation;
    Transform originalParent;
    int originalSiblingIndex;
    bool isDraggable;
    bool fromPlayerHand;
    bool fromSidePile;

    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void SetDraggable(bool canDrag)
    {
        isDraggable = canDrag;
    }

    public void SetOrigin(bool hand, bool side)
    {
        fromPlayerHand = hand;
        fromSidePile = side;
    }

    void AutoDetectOrigin(Transform parent)
    {
        if (parent == null) return;
        string n = parent.name.ToLower();
        if (n.Contains("playerhandzone"))
        {
            fromPlayerHand = true;
            fromSidePile = false;
            return;
        }
        if (n.Contains("side") && n.Contains("pile") && n.Contains("player"))
        {
            fromPlayerHand = false;
            fromSidePile = true;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        if (!fromPlayerHand && !fromSidePile) AutoDetectOrigin(transform.parent);
        if (!fromPlayerHand && !fromSidePile) return;

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
        if (!fromPlayerHand && !fromSidePile) return;
        if (rectTransform == null || canvas == null) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        if (!fromPlayerHand && !fromSidePile) return;

        canvasGroup.blocksRaycasts = true;
        GameObject hit = null;
        if (eventData.pointerCurrentRaycast.gameObject != null)
            hit = eventData.pointerCurrentRaycast.gameObject;

        Card thisCard = GetComponent<Card>();

        if (hit != null)
        {
            PlayFieldSlot slot = hit.GetComponentInParent<PlayFieldSlot>();
            if (slot != null && slot.CanPlace(thisCard))
            {
                slot.PlaceCard(thisCard, fromPlayerHand);
                bool emptied = RemoveFromOriginAndCheckEmptied();
                if (emptied && fromPlayerHand)
                    TurnManager.Instance.NotifyPlayedLastCardToField(true);
                return;
            }

            DiscardPile discard = hit.GetComponentInParent<DiscardPile>();
            if (discard != null)
            {
                if (CardWouldBePlayable(thisCard))
                {
                    ReturnToOriginalPosition();
                    return;
                }

                discard.AddCard(thisCard);
                bool emptied = RemoveFromOriginAndCheckEmptied();

                if (emptied && fromPlayerHand)
                {
                    Debug.Log("[CARDHANDLER] Player ha scartato l'ultima carta → notifico GameManager");
                    GameManager.Instance.OnPlayerDiscardLastCard();
                    return;
                }

                GameManager.Instance.EndPlayerTurn();
                return;
            }
        }

        ReturnToOriginalPosition();
    }

    bool RemoveFromOriginAndCheckEmptied()
    {
        bool emptied = false;
        HandManager handManager = Object.FindObjectOfType<HandManager>();
        SidePileManager sideManager = Object.FindObjectOfType<SidePileManager>();
        Card card = GetComponent<Card>();

        if (fromPlayerHand)
        {
            if (handManager != null)
            {
                handManager.RemoveCard(card);
                emptied = handManager.PlayerHand.Count == 0;
            }
        }
        else if (fromSidePile)
        {
            if (sideManager != null)
                sideManager.OnPlayerCardPlayedFromPile();
        }

        fromPlayerHand = false;
        fromSidePile = false;
        return emptied;
    }

    void ReturnToOriginalPosition()
    {
        rectTransform.SetParent(originalParent, false);
        rectTransform.SetSiblingIndex(originalSiblingIndex);
        rectTransform.position = originalPosition;
        rectTransform.rotation = originalRotation;
    }

    bool CardWouldBePlayable(Card card)
    {
        PlayFieldSlot[] slots = Object.FindObjectsOfType<PlayFieldSlot>();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].CanPlace(card)) return true;
        }
        return false;
    }
}
