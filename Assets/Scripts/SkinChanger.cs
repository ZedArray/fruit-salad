using UnityEngine;

public class SkinChanger : MonoBehaviour
{
    public AnimatorOverrideController[] skins;
    private int currentSkin = 0;
    public Animator anim;

    public void ChangeSkin()
    {
        currentSkin = (currentSkin + 1) % skins.Length;
        anim.runtimeAnimatorController = skins[currentSkin];
    }

}
