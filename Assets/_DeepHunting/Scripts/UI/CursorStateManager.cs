using System;
using System.Collections;
using System.Collections.Generic;
using InputIcons;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;

public class CursorStateManager : MMSingleton<CursorStateManager>
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (GameManager.Instance.Paused)
        {
            if (InputIconsManagerSO.CurrentInputDeviceIsKeyboard())
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
