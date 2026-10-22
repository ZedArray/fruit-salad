using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Prints credits.json as a shop receipt and slowly scrolls it; dragging pauses the scroll, tapping a link opens it.
// Sits on the ScrollRect so it gets the same drag/click events.
[RequireComponent(typeof(ScrollRect))]
public class CreditsReceipt : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IScrollHandler
{
    [SerializeField] private TextAsset creditsFile;
    [SerializeField] private TextMeshProUGUI receipt;
    // monospaced characters per line; must fit the text rect at the current font size
    [SerializeField] private int columns = 32;
    [SerializeField] private float scrollSpeed = 25f;
    // seconds to wait after the player scrolls (and at the start) before autoscrolling again
    [SerializeField] private float resumeDelay = 2f;

    private ScrollRect scroll;
    private bool dragging;
    private float lastTouch;

    void Start()
    {
        scroll = GetComponent<ScrollRect>();
        lastTouch = Time.time;
        var credits = JsonUtility.FromJson<CreditsParsed>(creditsFile.text);
        var sb = new StringBuilder("<mspace=0.5em>");
        int items = 0;

        Center(sb, Application.productName.ToUpper());
        Center(sb, Application.companyName.ToUpper());
        Center(sb, "*** CREDITS ***");
        sb.AppendLine();
        Row(sb, "2025-08-18", "18:08");
        Rule(sb, '-');

        foreach (var category in credits.categories)
        {
            sb.Append("<b>").Append(category.title.ToUpper()).AppendLine("</b>");
            foreach (var person in category.people)
            {
                if (person != category.people[0]) sb.AppendLine("<size=50%> </size>"); // half-line gap between items
                items++;
                if (string.IsNullOrEmpty(person.name)) Row(sb, person.alias, "");
                else Row(sb, person.name, person.alias);
                var details = person.additional.Select(note => "* " + note).ToList();
                if (person.links.Count > 0)
                    details.Insert(0, string.Join(" | ", person.links.Select(link => $"<link=\"{link}\"><u>{Label(link)}</u></link>")));
                // <indent> instead of spaces so wrapped lines stay indented;
                // closed before the last newline or it leaks onto the next name
                if (details.Count > 0) sb.Append("<indent=1em>").Append(string.Join("\n", details)).AppendLine("</indent>");
            }
            sb.AppendLine();
        }

        Rule(sb, '-');
        Row(sb, "ITEMS", items.ToString());
        sb.Append("<b>");
        Row(sb, "TOTAL", "Video Game");
        sb.Append("</b>");
        Rule(sb, '=');
        sb.AppendLine();
        Center(sb, "THANK YOU FOR PLAYING!");
        const string jam = "Play the original jam version";
        sb.Append(' ', (columns - jam.Length) / 2).AppendLine($"<link=\"https://graphicmismatch.itch.io/apple\"><u>{jam}</u></link>");
        receipt.text = sb.ToString();
    }

    void Update()
    {
        if (dragging || Time.time - lastTouch < resumeDelay || scroll.velocity.sqrMagnitude > 1f) return;
        var content = scroll.content;
        float bottom = content.rect.height - scroll.viewport.rect.height;
        var pos = content.anchoredPosition;
        if (pos.y < bottom) content.anchoredPosition = new Vector2(pos.x, Mathf.Min(pos.y + scrollSpeed * Time.deltaTime, bottom));
    }

    public void OnBeginDrag(PointerEventData eventData) => dragging = true;

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        lastTouch = Time.time;
    }

    public void OnScroll(PointerEventData eventData) => lastTouch = Time.time;

    public void OnPointerClick(PointerEventData eventData)
    {
        int link = TMP_TextUtilities.FindIntersectingLink(receipt, eventData.position, eventData.pressEventCamera);
        if (link >= 0) Application.OpenURL(receipt.textInfo.linkInfo[link].GetLinkID());
    }

    // "left.......right", or right on its own line when both don't fit
    void Row(StringBuilder sb, string left, string right)
    {
        int dots = columns - left.Length - right.Length;
        if (right.Length == 0) sb.AppendLine(left);
        else if (dots >= 2) sb.Append(left).Append('.', dots).AppendLine(right);
        else sb.AppendLine(left).AppendLine(right.PadLeft(columns));
    }

    void Center(StringBuilder sb, string text) => sb.AppendLine(text.PadLeft((columns + text.Length) / 2));

    void Rule(StringBuilder sb, char c) => sb.Append(c, columns).AppendLine();

    static readonly Dictionary<string, string> Sites = new()
    {
        { "itch.io", "itch.io" },
        { "github.com", "GitHub" },
        { "linkedin.com", "LinkedIn" },
        { "tumblr.com", "Tumblr" },
        { "tinder.com", "Tinder" },
    };

    // friendly name from the url's domain (subdomains ignored), "Website" for anything unknown
    static string Label(string url)
    {
        var host = new Uri(url).Host.Split('.');
        var domain = string.Join(".", host.Skip(Math.Max(0, host.Length - 2)));
        return Sites.TryGetValue(domain, out var name) ? name : "Website";
    }
}
