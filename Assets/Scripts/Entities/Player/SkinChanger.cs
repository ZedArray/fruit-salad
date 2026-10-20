using UnityEngine;

public class SkinChanger : MonoBehaviour
{
    public ShopCatalog catalog;
    public ShopCatalog tablecloths;
    public SpriteRenderer table;
    private int currentSkin = 0;
    public Animator anim;

    private void Awake()
    {
        var item = catalog.Find(UserManager.instance.getSkinID());
        currentSkin = System.Array.IndexOf(catalog.items, item);
        anim.runtimeAnimatorController = item.skin;
        table.sprite = tablecloths.Find(UserManager.instance.getTableID()).icon;
    }

    // debug: cycles through every skin, owned or not, without saving
    public void ChangeSkin()
    {
        currentSkin = (currentSkin + 1) % catalog.items.Length;
        anim.runtimeAnimatorController = catalog.items[currentSkin].skin;
    }
}
