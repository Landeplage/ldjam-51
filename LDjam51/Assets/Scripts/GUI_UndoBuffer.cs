using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AddRemoveAction
{
    public bool isAdd;
    public BlockOwner owner;
    public int second;
}

public class GUI_UndoBuffer : MonoBehaviour
{
    public RectTransform slider;
    public GUI_UndoBufferBlock blockPrefab;
    
    const float BLOCK_PX_SIZE = 80.0f;
    const float BLOCK_HALF_SPACING = 4.0f;

    int blockCount = 0;
    List<AddRemoveAction> actionQueue = new List<AddRemoveAction>();
    GUI_UndoBufferBlock pendingBlock;

    // Start is called before the first frame update
    void Start()
    {
        pendingBlock = AddBlock(BlockOwner.Player, ++blockCount);
        StartCoroutine(PollQueue());
    }

    public void QueueNextActionStart(BlockOwner owner)
    {
        actionQueue.Add(new AddRemoveAction() { isAdd = true, owner = owner, second = ++blockCount });
    }

    public void QueueRemovePlayerAction()
    {
        actionQueue.Add(new AddRemoveAction() { isAdd = false });
        actionQueue.Add(new AddRemoveAction() { isAdd = false });
        blockCount -= 2;
    }

    IEnumerator PollQueue()
    {
        while (true)
        {
            while (actionQueue.Count > 0)
            {
                var action = actionQueue[0];
                actionQueue.RemoveAt(0);
                if (action.isAdd)
                {
                    yield return AnimateAddBlock(action.owner, action.second);
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    yield return AnimateRemoveBlock();
                }
            }
            yield return null;
        }
    }

    IEnumerator AnimateRemoveBlock()
    {
        if (slider.childCount > 0)
        {
            Destroy(slider.GetChild(slider.childCount - 1).gameObject);
            yield return AnimateSlideOne(true);
        }
    }

    IEnumerator AnimateAddBlock(BlockOwner owner, int second)
    {
        var block = AddBlock(owner, second);
        yield return AnimateSlideOne(false);
    }
    
    GUI_UndoBufferBlock AddBlock(BlockOwner owner, int second)
    {
        var block = Instantiate(blockPrefab);
        block.Init(owner, second);
        block.gameObject.transform.parent = slider;
        block.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        return block;
    }
    
    IEnumerator AnimateSlideOne(bool slideLeft)
    {
        var start = slider.localPosition;
        var end = slider.localPosition + new Vector3((BLOCK_HALF_SPACING + BLOCK_PX_SIZE) * (slideLeft ? 1.0f : -1.0f), 0.0f, 0.0f);
        for (float i = 0; i < 1.0; i += 0.05f)
        {
            slider.localPosition = Vector3.Lerp(start, end, i);
            yield return null;
        }
        slider.localPosition = end;
    }
}
