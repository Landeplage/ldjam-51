using UnityEngine;
using FMODUnity;

public class MenuAudioPlayer : MonoBehaviour
{
    MenuController menuController;

    [SerializeField]
    EventReference menuEnter;

    private void Awake() {
        menuController = GetComponent<MenuController>();

        menuController.onGoTo.AddListener(OnGoTo);
    }

    void OnGoTo(string menu) {
        FMODUtility.Play(menuEnter);
    }
}
