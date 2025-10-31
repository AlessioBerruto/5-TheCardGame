using UnityEngine;
using UnityEngine.EventSystems;

public class PileSelector : MonoBehaviour, IPointerClickHandler
{
    public int pileIndex;
    private PlayerPickManager pickManager;

    private void Awake()
    {
        pickManager = FindObjectOfType<PlayerPickManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (pickManager != null)
        {
            pickManager.OnPlayerSelectPile(pileIndex);
        }
    }
}
