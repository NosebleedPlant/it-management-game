using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToDelete : MonoBehaviour,IPointerClickHandler
{
    private GameObject _parentFrame;
    private AudioSource[] _clickSfx;
    private void Start()
    {
        _parentFrame = transform.parent.gameObject;
        _clickSfx = GetComponentsInParent<AudioSource>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Destroy(_parentFrame);
        _clickSfx[1].Play();
    }
}
