using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetPositionZStaysZero : MonoBehaviour
{
    protected Transform _t;
    protected Vector3 _v = new Vector3(0f, 0f, 0f);

    private void Start()
    {
        _t = this.gameObject.transform;
    }

    private void Update()
    {
        if (!Mathf.Approximately(_t.position.z, 0f))
        {
            var position = _t.position;
            _v.x = position.x;
            _v.y = position.y;
            _t.position = _v;
        }
    }
}
