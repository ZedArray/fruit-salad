using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopCatalog[] catalogs;
    // one per catalog, same order; each shows only its catalog's rows
    [SerializeField] private Button[] tabs;
    [SerializeField] private Color inactiveTab = new Color(1, 1, 1, 0.5f);
    // cloned once per catalog item into the scrolling list; needs "Apple" (Image), "Name" (TMP), "Cost/Coin" (Image) and "Cost/Price" (TMP) children
    [SerializeField] private Button rowTemplate;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private Color deniedColor = new Color(0.8f, 0.2f, 0.15f);

    private Row[] rows;
    private Color coinTextColor;
    private Coroutine denying;
    private Row deniedRow;

    class Row
    {
        public ShopCatalog catalog;
        public ShopCatalog.Item item;
        public GameObject go;
        public RectTransform cost;
        public Vector2 costRest;
        public Image coin;
        public TextMeshProUGUI price;
    }

    void Start()
    {
        coinTextColor = coinText.color;
        var list = new List<Row>();
        foreach (var catalog in catalogs)
        foreach (var item in catalog.items)
        {
            var button = Instantiate(rowTemplate, rowTemplate.transform.parent);
            button.name = item.id;
            var icon = button.transform.Find("Apple").GetComponent<Image>();
            icon.sprite = item.icon;
            icon.preserveAspect = true; // tablecloths are 16:9
            button.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.name;
            var cost = (RectTransform)button.transform.Find("Cost");
            var row = new Row
            {
                catalog = catalog,
                item = item,
                go = button.gameObject,
                cost = cost,
                costRest = cost.anchoredPosition,
                coin = cost.Find("Coin").GetComponent<Image>(),
                price = cost.Find("Price").GetComponent<TextMeshProUGUI>(),
            };
            list.Add(row);
            button.onClick.AddListener(() => Buy(row));
        }
        rows = list.ToArray();
        rowTemplate.gameObject.SetActive(false);
        for (int i = 0; i < tabs.Length; i++)
        {
            int tab = i;
            tabs[i].onClick.AddListener(() => ShowTab(tab));
        }
        ShowTab(0);
        Refresh();
    }

    void ShowTab(int tab)
    {
        foreach (var row in rows) row.go.SetActive(row.catalog == catalogs[tab]);
        for (int i = 0; i < tabs.Length; i++) tabs[i].image.color = i == tab ? Color.white : inactiveTab;
        var scroll = rowTemplate.GetComponentInParent<ScrollRect>(true); // template is inactive by now
        LayoutRebuilder.ForceRebuildLayoutImmediate(scroll.content);
        scroll.verticalNormalizedPosition = 1;
    }

    void Buy(Row row)
    {
        if (!UserManager.instance.tryBuy(row.catalog, row.item))
        {
            Deny(row);
            return;
        }
        if (SFXManager.instance != null) SFXManager.instance.PlaySound("CoinPickup");
        AnalyticsManager.Record(new CustomEvent("itemPurchased")
        {
            { "itemId", row.item.id },
            { "price", row.item.price },
            { "isTablecloth", row.catalog.table },
        });
        Refresh();
    }

    // not enough coins: shake the price, flash the balance
    void Deny(Row row)
    {
        if (denying != null)
        {
            StopCoroutine(denying);
            deniedRow.cost.anchoredPosition = deniedRow.costRest;
        }
        deniedRow = row;
        denying = StartCoroutine(Shake(row));
    }

    IEnumerator Shake(Row row)
    {
        const float duration = 0.35f;
        coinText.color = deniedColor;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            row.cost.anchoredPosition = row.costRest + Vector2.right * (Mathf.Sin(t * 60f) * 8f * (1 - t / duration));
            yield return null;
        }
        row.cost.anchoredPosition = row.costRest;
        coinText.color = coinTextColor;
        denying = null;
    }

    void Refresh()
    {
        var user = UserManager.instance;
        coinText.text = user.getCoinAmount().ToString();
        foreach (var row in rows)
        {
            var item = row.item;
            bool owned = user.owns(item.id) || item.price == 0;
            row.coin.gameObject.SetActive(!owned);
            row.price.text = item == row.catalog.Find(user.getEquippedID(row.catalog)) ? "EQUIPPED"
                : owned ? "OWNED"
                : item.price.ToString();
        }
    }
}
