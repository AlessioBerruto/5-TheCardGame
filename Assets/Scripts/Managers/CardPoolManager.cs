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
        foreach (KeyValuePair<string, List<Sprite>> entry in suitSprites)
        {
            string suit = entry.Key;
            List<Sprite> sprites = entry.Value;

            if (suit == "joker")
            {
                for (int i = 0; i < sprites.Count; i++)
                {
                    GameObject obj = Instantiate(cardPrefab, poolParent);
                    obj.transform.localScale = Vector3.one;
                    Card c = obj.GetComponent<Card>();
                    c.Init(sprites[i], cardBackSprite, "joker", 0, true);
                    c.gameObject.SetActive(false);
                    pool.Add(c);
                }
                continue;
            }

            for (int i = 0; i < sprites.Count * 2; i++)
            {
                int rank = (i % sprites.Count) + 1;
                if (rank == 1) continue;

                GameObject obj = Instantiate(cardPrefab, poolParent);
                obj.transform.localScale = Vector3.one;
                Card c = obj.GetComponent<Card>();
                c.Init(sprites[i % sprites.Count], cardBackSprite, suit, rank);
                c.gameObject.SetActive(false);
                pool.Add(c);
            }
        }
    }

    public List<Card> GetAces()
    {
        List<Card> aces = new List<Card>();
        string[] suits = new string[] { "club", "diamond", "heart", "spade" };

        foreach (string suit in suits)
        {
            Sprite aceSprite = suitSprites[suit].FirstOrDefault(s => s.name == "1_" + suit);
            if (aceSprite == null)
            {
                Debug.LogWarning("Asso non trovato per il seme " + suit);
                continue;
            }

            for (int i = 0; i < 2; i++)
            {
                GameObject obj = Instantiate(cardPrefab, poolParent);
                obj.transform.localScale = Vector3.one;
                Card c = obj.GetComponent<Card>();
                c.Init(aceSprite, cardBackSprite, suit, 1);
                aces.Add(c);
            }
        }

        return aces;
    }

    public List<Card> GetAllNonAceCards()
    {
        return new List<Card>(pool);
    }

    public Card GetCard()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            Card c = pool[i];
            if (!c.gameObject.activeInHierarchy)
            {
                c.gameObject.SetActive(true);
                return c;
            }
        }
        Debug.LogWarning("Card pool esaurito!");
        return null;
    }

    public void ReturnCard(Card card)
    {
        card.ResetCard();
    }
}
