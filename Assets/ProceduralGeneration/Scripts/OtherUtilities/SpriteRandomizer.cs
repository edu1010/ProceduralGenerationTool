using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class SpriteRandomizer : MonoBehaviour
{
    [SerializeField]private Sprite[] _sprites;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _sprites[Random.Range(0, _sprites.Length)];
    }

}
