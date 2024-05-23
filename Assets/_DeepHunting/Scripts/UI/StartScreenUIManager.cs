
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.Tools;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StartScreenUIManager : MonoBehaviour, MMEventListener<MMGameEvent>
{
    [Header("UI Controller")]
    public UIControllerSO uiController;
    
    [Header("Panels")]
    [MMReadOnly] public CanvasGroup TitlePanel;
    [MMReadOnly] public CanvasGroup IndicatorPanel;
    [MMReadOnly] public CanvasGroup ButtonPanel;
    [MMReadOnly] public CanvasGroup OptionPanel;

    [Header("Buttons")]
    [MMReadOnly] public Button IndicatorBtn;
    [MMReadOnly] public Button ContinueBtn;
    [MMReadOnly] public Button NewGameBtn;
    [MMReadOnly] public Button OptionsBtn;
    [MMReadOnly] public Button CreditsBtn;
    [MMReadOnly] public Button QuitBtn;
    [MMReadOnly] public Button BackgroundBtn;

    [Header("Icons")]
    [MMReadOnly] public TextMeshProUGUI ContinueIcon;
    [MMReadOnly] public TextMeshProUGUI NewGameIcon;
    [MMReadOnly] public TextMeshProUGUI OptionsIcon;
    [MMReadOnly] public TextMeshProUGUI CreditsIcon;
    [MMReadOnly] public TextMeshProUGUI QuitIcon;
    
    [Header("Playable Assets")]
    [MMReadOnly] public PlayableDirector SplashPlayableAsset;
    [MMReadOnly] public PlayableDirector IndicatorPlayableAsset;
    
    
    protected Button _preBtn = null;
    protected Button _defaultStartBtn = null;
    protected Navigation _curBackgroundBtnNavigation;
    
    private void Start()
    {
        TitlePanel.gameObject.SetActive(false);
        IndicatorPanel.gameObject.SetActive(false);
        ButtonPanel.gameObject.SetActive(false);
        TitlePanel.GetComponent<CanvasGroup>().alpha = 0;
        IndicatorPanel.GetComponent<CanvasGroup>().alpha = 0;
        ButtonPanel.GetComponent<CanvasGroup>().alpha = 0;

        IndicatorPanel.interactable = false;
        ButtonPanel.interactable = false;

        IndicatorPanel.blocksRaycasts = false;
        ButtonPanel.blocksRaycasts = false;
        
        SplashPlayableAsset.gameObject.SetActive(true);
        IndicatorPlayableAsset.gameObject.SetActive(false);
        
        ContinueIcon.gameObject.SetActive(false);
        NewGameIcon.gameObject.SetActive(false);
        OptionsIcon.gameObject.SetActive(false);
        CreditsIcon.gameObject.SetActive(false);
        QuitIcon.gameObject.SetActive(false);
    }

    public void TitleShowUp()
    {
        PanelShowUp(TitlePanel, SceneControllerGetValue("TitleShowUp"));
    }
    
    public void IndicatorShowUp()
    {
        PanelShowUp(IndicatorPanel, SceneControllerGetValue("IndicatorShowUp"));
        
        EventSystem.current.SetSelectedGameObject(IndicatorBtn.gameObject, null);
    }
    
    public void OnClickIndicator()
    {
        PanelDisappear(TitlePanel, SceneControllerGetValue("TitleAndIndicatorDisappear"));
        PanelDisappear(IndicatorPanel, SceneControllerGetValue("TitleAndIndicatorDisappear"));
        
        IndicatorPlayableAsset.gameObject.SetActive(true);
        IndicatorPlayableAsset.Play();
    }

    public virtual void OnMMEvent(MMGameEvent e)
    {
        if (e.EventName == "OnBackFromOptions")
        {
            ButtonPanelShowUpFromOptions();
        }
    }

    public void ButtonPanelShowUpFromIndicator()
    {
        if (GetSaveData())
        {
            ContinueBtn.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(ContinueBtn.gameObject, null);
            
            _defaultStartBtn = ContinueBtn;
            _preBtn = ContinueBtn;
            
            Navigation n = new Navigation();
            n.mode = Navigation.Mode.Explicit;
            n.selectOnUp = ContinueBtn;
            n.selectOnDown = OptionsBtn;
            NewGameBtn.navigation = n;
            
            ContinueIcon.gameObject.SetActive(true);
            
            UpdateBackgroundBtnNavigation();
        }
        else
        {
            ContinueBtn.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(NewGameBtn.gameObject, null);
            
            _defaultStartBtn = NewGameBtn;
            _preBtn = NewGameBtn;
            
            Navigation n = new Navigation();
            n.mode = Navigation.Mode.Explicit;
            n.selectOnUp = NewGameBtn;
            n.selectOnDown = OptionsBtn;
            NewGameBtn.navigation = n;
            
            NewGameIcon.gameObject.SetActive(true);
            
            UpdateBackgroundBtnNavigation();
        }
        
        PanelShowUp(ButtonPanel, SceneControllerGetValue("ButtonPanelShowUpFromIndicator"));
    }
    
    public void ButtonPanelShowUpFromOptions()
    {
        if (GetSaveData())
        {
            ContinueBtn.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(ContinueBtn.gameObject, null);
            
            _defaultStartBtn = ContinueBtn;
            _preBtn = ContinueBtn;
            
            Navigation n = new Navigation();
            n.mode = Navigation.Mode.Explicit;
            n.selectOnUp = ContinueBtn;
            n.selectOnDown = OptionsBtn;
            NewGameBtn.navigation = n;
            
            ContinueIcon.gameObject.SetActive(true);
            
            UpdateBackgroundBtnNavigation();
        }
        else
        {
            ContinueBtn.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(NewGameBtn.gameObject, null);
            
            _defaultStartBtn = NewGameBtn;
            _preBtn = NewGameBtn;
            
            Navigation n = new Navigation();
            n.mode = Navigation.Mode.Explicit;
            n.selectOnUp = NewGameBtn;
            n.selectOnDown = OptionsBtn;
            NewGameBtn.navigation = n;
            
            NewGameIcon.gameObject.SetActive(true);
            
            UpdateBackgroundBtnNavigation();
        }
        
        PanelShowUp(ButtonPanel, SceneControllerGetValue("ButtonPanelShowUpFromOptions"));
    }

    #region Button

    public void OnClickContinue()
    {
        Debug.Log("Continue");
    }
    
    public void OnClickNewGame()
    {
        Debug.Log("NewGame");
    }
    
    public void OnClickOptions()
    {
        Debug.Log("Options");
        
        PanelDisappear(ButtonPanel, SceneControllerGetValue("ButtonPanelDisappearToOptions"));
        MMEventManager.TriggerEvent(new MMGameEvent("OnOpenOptionsPanel"));
    }
    
    public void OnClickCredits()
    {
        Debug.Log("Credits");
    }
    
    public void OnClickQuit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void OnClickBackground()
    {
        Debug.Log("Background");
    }

    #endregion

    private void Update()
    {
        if (_preBtn == null) return;
        
        if (EventSystem.current.currentSelectedGameObject != _preBtn.gameObject)
        {
            ContinueIcon.gameObject.SetActive(false);
            NewGameIcon.gameObject.SetActive(false);
            OptionsIcon.gameObject.SetActive(false);
            CreditsIcon.gameObject.SetActive(false);
            QuitIcon.gameObject.SetActive(false);
            if (EventSystem.current.currentSelectedGameObject == ContinueBtn.gameObject)
            {
                _preBtn = ContinueBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == NewGameBtn.gameObject)
            {
                _preBtn = NewGameBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == OptionsBtn.gameObject)
            {
                _preBtn = OptionsBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == CreditsBtn.gameObject)
            {
                _preBtn = CreditsBtn;
            }
            else if (EventSystem.current.currentSelectedGameObject == QuitBtn.gameObject)
            {
                _preBtn = QuitBtn;
            }
            UpdateBackgroundBtnNavigation();
        }
        
        if (EventSystem.current.currentSelectedGameObject == ContinueBtn.gameObject)
        {
            ContinueIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == NewGameBtn.gameObject)
        {
            NewGameIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == OptionsBtn.gameObject)
        {
            OptionsIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == CreditsBtn.gameObject)
        {
            CreditsIcon.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == QuitBtn.gameObject)
        {
            QuitIcon.gameObject.SetActive(true);
        }
    }

    private void UpdateBackgroundBtnNavigation()
    {
        _curBackgroundBtnNavigation = _preBtn.navigation;
        _curBackgroundBtnNavigation.mode = Navigation.Mode.Explicit;
        BackgroundBtn.navigation = _curBackgroundBtnNavigation;
    }

    private bool GetSaveData()
    {
        if (ES3.KeyExists("FirstPlay"))
        {
            bool b = ES3.Load<bool>("FirstPlay");
            return b;
        }
        
        return false;
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
