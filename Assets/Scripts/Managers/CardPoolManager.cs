using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardPoolManager : MonoBehaviour
{
    public static CardPoolManager Instance { get; private set; }

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform poolParent;

    private List<Card> pool = new List<Card>();
    private Sprite cardBackSprite;
    private Dictionary<string, List<Sprite>> suitSprites = new Dictionary<string, List<Sprite>>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadSprites();
        BuildPool();
    }

    private void LoadSprites()
    {
        suitSprites["club"] = Resources.LoadAll<Sprite>("Cards/Fronts/club").ToList();
        suitSprites["spade"] = Resources.LoadAll<Sprite>("Cards/Fronts/spade").ToList();
        suitSprites["heart"] = Resources.LoadAll<Sprite>("Cards/Fronts/heart").ToList();
        suitSprites["diamond"] = Resources.LoadAll<Sprite>("Cards/Fronts/diamond").ToList();
        suitSprites["joker"] = Resources.LoadAll<Sprite>("Cards/Fronts/joker").ToList();
        cardBackSprite = Resources.Load<Sprite>("Cards/Back/CardBack");
    }

    private void BuildPool()
    {
        pool.Clear();

        string[] suits = new string[] { "club", "diamond", "heart", "spade" };

        foreach (string suit in suits)
        {
            List<Sprite> sprites = suitSprites[suit];
            int ranksCount = sprites.Count;

            for (int copy = 0; copy < 2; copy++)
            {
                for (int i = 0; i < ranksCount; i++)
                {
                    int rank = ParseRankFromSpriteName(sprites[i].name, i + 1);
                    CreateAndAddCard(suit, sprites[i], rank, false);
                }
            }
        }

        List<Sprite> jokerSprites = suitSprites["joker"];
        for (int i = 0; i < jokerSprites.Count; i++)
            CreateAndAddCard("joker", jokerSprites[i], 0, true);

        foreach (Card c in pool)
            c.gameObject.SetActive(false);
    }

    private int ParseRankFromSpriteName(string spriteName, int fallback)
    {
        if (string.IsNullOrEmpty(spriteName)) return fallback;
        string[] parts = spriteName.Split('_');
        int parsed;
        if (parts.Length > 0 && int.TryParse(parts[0], out parsed))
            return parsed;
        if (spriteName.Length >= 2)
        {
            string num = "";
            for (int i = 0; i < spriteName.Length; i++)
            {
                char ch = spriteName[i];
                if (char.IsDigit(ch)) num += ch;
                else if (num.Length > 0) break;
            }
            if (num.Length > 0 && int.TryParse(num, out parsed)) return parsed;
        }
        return fallback;
    }


    private void CreateAndAddCard(string suit, Sprite front, int rank, bool isJoker)
    {
        GameObject obj = Instantiate(cardPrefab, poolParent);
        obj.transform.localScale = Vector3.one;
        Card c = obj.GetComponent<Card>();
        c.Init(front, cardBackSprite, suit, rank, isJoker);
        pool.Add(c);
    }

    public List<Card> GetAces()
    {
        List<Card> aces = new List<Card>();
        string[] suits = new string[] { "club", "diamond", "heart", "spade" };

        foreach (string suit in suits)
        {
            for (int i = 0; i < 2; i++)
            {
                Card found = pool.Find(x => x.Suit == suit && x.Rank == 1 && !x.gameObject.activeInHierarchy && !x.IsJoker);
                if (found != null)
                {
                    found.gameObject.SetActive(true);
                    aces.Add(found);
                }
                else
                {
                    Debug.LogWarning("CardPoolManager: asso non trovato per seme " + suit);
                }
            }
        }

        return aces;
    }

    public List<Card> GetAllNonAceCards()
    {
        List<Card> copy = pool.Where(x => x.Rank != 1 && !x.IsJoker && !x.gameObject.activeInHierarchy).ToList();
        return copy;
    }

    public Card GetCard()
    {
        Card found = pool.Find(x => !x.gameObject.activeInHierarchy);
        if (found != null)
        {
            found.gameObject.SetActive(true);
            return found;
        }
        Debug.LogWarning("CardPoolManager: pool esaurito");
        return null;
    }

    public void ReturnCard(Card card)
    {
        if (card == null) return;
        card.ResetCard();
    }

    public Transform GetPoolParent()
    {
        return poolParent;
    }
}
