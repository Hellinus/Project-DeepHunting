using System;
using System.Collections;
using System.Collections.Generic;
using InputIcons;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;

public class StartScreenCursorStateManager : MMSingleton<StartScreenCursorStateManager>
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Debug.Log(InputIconsManagerSO.CurrentInputDeviceIsKeyboard());
        if (InputIconsManagerSO.CurrentInputDeviceIsKeyboard())
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }
    }
}
