using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player Player = new ();
    public Camera Camera { get; set; }

    [SerializeField] private List<Minion> controlledMinions = new ();
    [SerializeField] private List<Champion> controlledChampions = new ();

    private void Awake()
    {
        Camera ??= Camera.main;
    }

    private void Update()
    {
        if (!Player.IsTurnActive)
            return;
        
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition); 
            if (Physics.Raycast(ray, out var hit)) 
            {
                Debug.Log("Clicked collider of " + hit.transform.name);
            }
        }
    }
}