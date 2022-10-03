using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BlockOwner
{
    Player,
    AI,
    None
}

public class GUI_UndoBufferBlock : MonoBehaviour
{
    public TMP_Text text;
    public Image image;

    public Sprite playerBgSprite;
    public Sprite aiBgSprite;
    public Sprite noneBgSprite;

    public void Init(BlockOwner owner, int turn)
    {
        SetBlockOwner(owner);
        text.text = turn + "<size=70%>s";
    }

    public void SetBlockOwner(BlockOwner owner)
    {
        switch (owner)
        {
            case BlockOwner.Player: image.sprite = playerBgSprite; break;
            case BlockOwner.AI: image.sprite = aiBgSprite; break;
            case BlockOwner.None: // fall-through
            default: image.sprite = noneBgSprite; break;
        }
    }
}
