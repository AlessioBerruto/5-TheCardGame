using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Card : MonoBehaviour
{
    [SerializeField] private Image frontImage;
    [SerializeField] private Image backImage;

    public string Suit { get; private set; }
    public int Rank { get; private set; }
    public bool IsFaceUp { get; private set; }
    public bool IsJoker { get; private set; }

    private RectTransform rect;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Init(Sprite front, Sprite back, string suit, int rank, bool isJoker = false)
    {
        frontImage.sprite = front;
        backImage.sprite = back;
        Suit = suit;
        Rank = rank;
        IsJoker = isJoker;
        Flip(false);
    }

    public void Flip(bool faceUp)
    {
        IsFaceUp = faceUp;
        frontImage.gameObject.SetActive(faceUp);
        backImage.gameObject.SetActive(!faceUp);
    }

    public void ResetCard()
    {
        Flip(false);
        transform.SetParent(CardPoolManager.Instance.transform);
        gameObject.SetActive(false);
    }

    public void MoveTo(Vector3 target, float speed = 10f)
    {
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * speed);
    }

    public bool IsConsecutiveTo(Card other)
    {
        if (other == null) return false;
        if (IsJoker || other.IsJoker) return true;
        if (Suit != other.Suit) return false;
        return Rank == other.Rank + 1;
    }
}
