using System;
using System.Collections;
using System.Collections.Generic;
using Fluxy;
using UnityEngine;
using UnityEngine.Serialization;

public class FluDensityTextureUpdate : MonoBehaviour
{ 
    public SpriteRenderer CharacerSpriteRender;
    protected FluxyTarget _fluTarget;

    protected Sprite _sprite;
    protected Rect _rect;
    protected Texture2D _texture;

    private void Start()
    {
        _fluTarget = GetComponent<FluxyTarget>();

    }

    void Update()
    {
        _sprite = CharacerSpriteRender.sprite;
        
        _rect = _sprite.rect;
        _texture = new Texture2D((int)_rect.width, (int)_rect.height);
 
        _texture.SetPixels(0, 0, (int)_rect.width, (int)_rect.height, _sprite.texture.GetPixels((int)_rect.x, (int)_rect.y, (int)_rect.width, (int)_rect.height));
        _texture.Apply();
        
        _fluTarget.densityTexture = _texture;
    }
}
