using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameManager mainManager;
    [SerializeField] private Storage_SubManager StorageMiniGame;
    [SerializeField] private Network_SubManager NetwrokMiniGame;
    [SerializeField] private Security_SubManager SecurityMiniGame;
    [SerializeField] private GameObject Canvas;
    private InputActions playerControls;
    
    private bool _held = false;
    private PointerEventData _mouseEventData;
    private GraphicRaycaster _uiRaycaster;
    private List<RaycastResult> _click_results;

    private void Start()
    {
        _mouseEventData = new PointerEventData(EventSystem.current);
        _uiRaycaster = Canvas.GetComponent<GraphicRaycaster>();
        _click_results = new List<RaycastResult>();
    }

    public void MouseEvent()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GameInputRecived(NetwrokMiniGame.checkMaking);
            GameInputRecived(StorageMiniGame.movePlayerObject);
        }
        if(Input.GetMouseButton(0))
        {
            GameInputRecived(NetwrokMiniGame.dragWire);
            GameInputRecived(SecurityMiniGame.movePlayerObject);
        }
        if(Input.GetMouseButtonUp(0))
        {
            NetwrokMiniGame.ClearConnection();
        }
    }

    private void GameInputRecived(Action<Vector3> func)
    {
        _mouseEventData.position = Mouse.current.position.ReadValue();
        _click_results.Clear();
        _uiRaycaster.Raycast(_mouseEventData, _click_results);
        foreach(RaycastResult result in _click_results)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(_mouseEventData.position);
            if (result.gameObject.tag == "MinigameWindow")
            {
                func(worldPosition);
            }
        }
    }
}
