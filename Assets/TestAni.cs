using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAni : MonoBehaviour
{
    public SpriteAnimation spriteAni;
    public SpriteAnimationManager.AtlasAniType type;
    public TestName characterName;
    public enum TestName
    {
        Bulgom,
        ButterusIII,
        CarnarJungtur,
        DdongGangAJi,
        Dulgi,
        Gosegu,
        Ine,
        Jentoo,
        JingBurger,
        Jururu,
        Lilpa,
        Panchi,
        PungSin,
        Secretto,
        SuSaemi,
        Viichan,
    }



    public void PlayAni()
    {
        spriteAni.PlayAnimation(type, characterName.ToString(), true, 0);
    }
}
