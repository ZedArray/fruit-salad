using UnityEngine;

public class SkinChanger : MonoBehaviour
{
    public AnimatorOverrideController[] skins;
    private int currentSkin = 0;
    public Animator anim;

    private void Awake()
    {
        currentSkin = UserManager.instance.getSkinID();
        changeToID();
    }

    private void changeToID()
    {
        anim.runtimeAnimatorController = skins[currentSkin];
    }

    public void ChangeSkin()
    {
        currentSkin = (currentSkin + 1) % skins.Length;
        anim.runtimeAnimatorController = skins[currentSkin];
    }

}
