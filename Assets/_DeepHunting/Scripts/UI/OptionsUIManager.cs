using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.Tools;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OptionsUIManager : MonoBehaviour, MMEventListener<MMGameEvent>
{
    [FormerlySerializedAs("SceneController")] [Header("SceneController")]
    public UIControllerSO uiController;
    
    [Header("Panels")]
    public CanvasGroup OptionsPanel;

    [Header("Buttons")]
    public Button MouseSensitivityDecreaseBtn;
    public Button MouseSensitivityIncreaseBtn;
    
    public Button GamepadSensitivityDecreaseBtn;
    public Button GamepadSensitivityIncreaseBtn;
    
    public Button MasterVolumeDecreaseBtn;
    public Button MasterVolumeIncreaseBtn;
    public Button MusicVolumeDecreaseBtn;
    public Button MusicVolumeIncreaseBtn;
    public Button SFXVolumeDecreaseBtn;
    public Button SFXVolumeIncreaseBtn;
    
    public Button LanguageSwitchLeftBtn;
    public Button LanguageSwitchRightBtn;
    
    public Button BackBtn;
    
    [Header("BackgroundButton")]
    public List<Button> BackgroundBtnList;

    [Header("VolumeText")]
    public TextMeshProUGUI MouseSensitivityText;
    public TextMeshProUGUI GamepadSensitivityText;
    
    public TextMeshProUGUI MasterVolumeText;
    public TextMeshProUGUI MusicVolumeText;
    public TextMeshProUGUI SFXVolumeText;

    [Header("Icons")]
    public TextMeshProUGUI MouseSensitivityIcon;
    public TextMeshProUGUI GamepadSensitivityIcon;
    
    public TextMeshProUGUI MasterVolumeIcon;
    public TextMeshProUGUI MusicVolumeIcon;
    public TextMeshProUGUI SFXVolumeIcon;
    public TextMeshProUGUI LanguageIcon;
    public TextMeshProUGUI BackIcon;

    [Header("LanguageList")]
    public LanuageListSO LanguageList;
    
    protected string _defaultDashLine = "-------";
    protected string _variantDashLine = "--------";
    protected int _curLanguageIndex = 0;

    protected Button _preBtn = null;
    protected Button _defaultStartBtn = null;
    protected Navigation _curBackgroundBtnNavigation;
    
    private void Start()
    {
        _defaultStartBtn = MouseSensitivityDecreaseBtn;
        _preBtn = MouseSensitivityDecreaseBtn;
        
        MouseSensitivityIcon.gameObject.SetActive(false);
        GamepadSensitivityIcon.gameObject.SetActive(false);
        MasterVolumeIcon.gameObject.SetActive(false);
        MusicVolumeIcon.gameObject.SetActive(false);
        SFXVolumeIcon.gameObject.SetActive(false);
        LanguageIcon.gameObject.SetActive(false);
        BackIcon.gameObject.SetActive(false);

        OptionsPanel.interactable = false;
        OptionsPanel.blocksRaycasts = false;
        
        switch (I2.Loc.LocalizationManager.CurrentLanguage)
        {
            case "English":
                _curLanguageIndex = 0;
                break;
            case "Chinese (Simplified)":
                _curLanguageIndex = 1;
                break;
            case "Chinese (Traditional)":
                _curLanguageIndex = 2;
                break;
        }
    }
    
    public virtual void OnMMEvent(MMGameEvent e)
    {
        if (e.EventName == "OnOpenOptionsPanel")
        {
            OptionsPanelShowUp();
        }
    }
    
    public void OptionsPanelShowUp()
    {
        PanelShowUp(OptionsPanel, SceneControllerGetValue("OptionsPanelShowUp"));
        
        EventSystem.current.SetSelectedGameObject(_defaultStartBtn.gameObject, null);
        UpdateBackgroundBtnNavigation();
    }

    #region Button

    public void OnMouseSensitivityDecreaseBtn()
    {
        Debug.Log("MouseSensitivityDecrease");
        
    }
    
    public void OnMouseSensitivityIncreaseBtn()
    {
        Debug.Log("MouseSensitivityIncrease");
        
    }
    
    public void OnGamepadSensitivityDecreaseBtn()
    {
        Debug.Log("GamepadSensitivityDecrease");
        
    }
    
    public void OnGamepadSensitivityIncreaseBtn()
    {
        Debug.Log("GamepadSensitivityIncrease");
        
    }
    
    public void OnMasterVolumeDecreaseBtn()
    {
        Debug.Log("MasterVolumeDecrease");
        
    }
    
    public void OnMasterVolumeIncreaseBtn()
    {
        Debug.Log("MasterVolumeIncrease");
        
    }
    
    public void OnMusicVolumeDecreaseBtn()
    {
        Debug.Log("MusciVolumeDecrease");
        
    }
    
    public void OnMusicVolumeIncreaseBtn()
    {
        Debug.Log("MusicVolumeIncrease");
        
    }
    
    public void OnSFXVolumeDecreaseBtn()
    {
        Debug.Log("SFXVolumeDecrease");
        
    }
    
    public void OnSFXVolumeIncreaseBtn()
    {
        Debug.Log("SFXVolumeIncrease");
        
    }

    public void OnLanguageSwitchLeftBtn()
    {
        _curLanguageIndex -= 1;
        if (_curLanguageIndex < 0)
        {
            _curLanguageIndex = LanguageList.LanguageList.Count - 1;
            if (I2.Loc.LocalizationManager.HasLanguage(LanguageList.LanguageList[_curLanguageIndex]))
            {
                I2.Loc.LocalizationManager.CurrentLanguage = LanguageList.LanguageList[_curLanguageIndex];
            }
        }
        else
        {
            if (I2.Loc.LocalizationManager.HasLanguage(LanguageList.LanguageList[_curLanguageIndex]))
            {
                I2.Loc.LocalizationManager.CurrentLanguage = LanguageList.LanguageList[_curLanguageIndex];
            }
        }
        
        Debug.Log("LanguageSwitchLeft");
    }
    
    public void OnLanguageSwitchRightBtn()
    {
        _curLanguageIndex += 1;
        if (LanguageList.LanguageList.Count > _curLanguageIndex)
        {
            if (I2.Loc.LocalizationManager.HasLanguage(LanguageList.LanguageList[_curLanguageIndex]))
            {
                I2.Loc.LocalizationManager.CurrentLanguage = LanguageList.LanguageList[_curLanguageIndex];
            }
        }
        else
        {
            _curLanguageIndex = 0;
            if (I2.Loc.LocalizationManager.HasLanguage(LanguageList.LanguageList[_curLanguageIndex]))
            {
                I2.Loc.LocalizationManager.CurrentLanguage = LanguageList.LanguageList[_curLanguageIndex];
            }
        }
        Debug.Log("LanguageSwitchRight");
    }
    
    public void OnBackBtn()
    {
        Debug.Log("Back");
        
        PanelDisappear(OptionsPanel, SceneControllerGetValue("OptionsPanelDisappear"));
        
        MMEventManager.TriggerEvent(new MMGameEvent("OnBackFromOptions"));
    }

    #endregion

    private void Update()
    {
        if (_preBtn == null) return;
        
        if (EventSystem.current.currentSelectedGameObject != _preBtn.gameObject)
        {
            MouseSensitivityIcon.gameObject.SetActive(false);
            GamepadSensitivityIcon.gameObject.SetActive(false);
            MasterVolumeIcon.gameObject.SetActive(false);
            MusicVolumeIcon.gameObject.SetActive(false);
            SFXVolumeIcon.gameObject.SetActive(false);
            LanguageIcon.gameObject.SetActive(false);
            BackIcon.gameObject.SetActive(false);
            if (EventSystem.current.currentSelectedGameObject == MouseSensitivityDecreaseBtn.gameObject)
            {
                _preBtn = MouseSensitivityDecreaseBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == MouseSensitivityIncreaseBtn.gameObject)
            {
                _preBtn = MouseSensitivityIncreaseBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == GamepadSensitivityDecreaseBtn.gameObject)
            {
                _preBtn = GamepadSensitivityDecreaseBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == GamepadSensitivityIncreaseBtn.gameObject)
            {
                _preBtn = GamepadSensitivityIncreaseBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == MasterVolumeDecreaseBtn.gameObject)
            {
                _preBtn = MasterVolumeDecreaseBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == MasterVolumeIncreaseBtn.gameObject)
            {
                _preBtn = MasterVolumeIncreaseBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == MusicVolumeDecreaseBtn.gameObject)
            {
                _preBtn = MusicVolumeDecreaseBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == MusicVolumeIncreaseBtn.gameObject)
            {
                _preBtn = MusicVolumeIncreaseBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == SFXVolumeDecreaseBtn.gameObject)
            {
                _preBtn = SFXVolumeDecreaseBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == SFXVolumeIncreaseBtn.gameObject)
            {
                _preBtn = SFXVolumeIncreaseBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == LanguageSwitchLeftBtn.gameObject)
            {
                _preBtn = LanguageSwitchLeftBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == LanguageSwitchRightBtn.gameObject)
            {
                _preBtn = LanguageSwitchRightBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == BackBtn.gameObject)
            {
                _preBtn = BackBtn;
            }
        
            UpdateBackgroundBtnNavigation();
        }
        
        if (EventSystem.current.currentSelectedGameObject == MouseSensitivityDecreaseBtn.gameObject)
        {
            MouseSensitivityIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == MouseSensitivityIncreaseBtn.gameObject)
        {
            MouseSensitivityIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == GamepadSensitivityDecreaseBtn.gameObject)
        {
            GamepadSensitivityIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == GamepadSensitivityIncreaseBtn.gameObject)
        {
            GamepadSensitivityIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == MasterVolumeDecreaseBtn.gameObject)
        {
            MasterVolumeIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == MasterVolumeIncreaseBtn.gameObject)
        {
            MasterVolumeIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == MusicVolumeDecreaseBtn.gameObject)
        {
            MusicVolumeIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == MusicVolumeIncreaseBtn.gameObject)
        {
            MusicVolumeIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == SFXVolumeDecreaseBtn.gameObject)
        {
            SFXVolumeIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == SFXVolumeIncreaseBtn.gameObject)
        {
            SFXVolumeIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == LanguageSwitchLeftBtn.gameObject)
        {
            LanguageIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == LanguageSwitchRightBtn.gameObject)
        {
            LanguageIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == BackBtn.gameObject)
        {
            BackIcon.gameObject.SetActive(true);
        }
    }

    private void UpdateBackgroundBtnNavigation()
    {
        _curBackgroundBtnNavigation = _preBtn.navigation;
        _curBackgroundBtnNavigation.mode = Navigation.Mode.Explicit;
        foreach (var btn in BackgroundBtnList)
        {
            btn.navigation = _curBackgroundBtnNavigation;
        }
    }
    
    protected void PanelShowUp(CanvasGroup c, float f)
    {
        c.gameObject.SetActive(true);
        c.interactable = true;
        c.blocksRaycasts = true;
        
        c.DOKill();
        c.DOFade(1, f);
    }
    
    protected void PanelDisappear(CanvasGroup c, float f)
    {
        c.interactable = false;
        c.blocksRaycasts = false;
        
        c.DOKill();
        c.DOFade(0, f);
    }

    private bool GetSaveData()
    {
        // todo: easy save here.
        return false;
    }
    
    protected float SceneControllerGetValue(string id)
    {
        foreach (var VARIABLE in uiController.Parameters)
        {
            if (VARIABLE.Id == id)
            {
                return VARIABLE.Value;
            }
        }
            
        Debug.LogAssertion("SceneController - " + uiController.name + ": Lost Id - " + id);
        return uiController.DefaultValue;
    }

    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
    }
    
    private void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
    }
}
