using SystemUtil;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainUISystem : MonoBehaviour
{
    [SerializeField]
    private MainSystem mainSystem;
    [SerializeField]
    private GameObject keyboard;
    [SerializeField]
    private GameObject settings;

    public void SetKeyboardPanelVisible()
    {
        settings.SetActive(false);
        keyboard.SetActive(true);
        mainSystem.SetCurrentPanel(MainSystemUtil.Panel.KEYBOARD);
    }

    public void SetSettingPanelVisible()
    {
        keyboard.SetActive(false);
        settings.SetActive(true);
        mainSystem.SetCurrentPanel(MainSystemUtil.Panel.SETTING);
    }
}
