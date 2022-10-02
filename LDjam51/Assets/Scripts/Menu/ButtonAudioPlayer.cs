using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

namespace S13Audio.PNRR
{
    public class ButtonAudioPlayer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField]
        EventReference onHighlight;

        [SerializeField]
        EventReference onPressed;

        [SerializeField]
        EventReference onPressedExtra;

        public void OnPointerEnter(PointerEventData data) {
            FMODUtility.Play(onHighlight);
        }

        public void OnPointerClick(PointerEventData data) {
            FMODUtility.Play(onPressed);
            FMODUtility.Play(onPressedExtra);
        }
    }
}
