using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountainerCounterVisual : MonoBehaviour
{

    private const String OPEN_CLOSE = "OpenClose";

    [SerializeField] private ContainerCounter containerCounter;
    private Animator animator;
   
    private void Awake(){
        animator = GetComponent<Animator>();
        containerCounter.OnPlayerGrabbedObject += ContainerCounter_OnPlayerGrabbedObject;
    }

    private void ContainerCounter_OnPlayerGrabbedObject(object sender, EventArgs e)
    {
        animator.SetTrigger(OPEN_CLOSE);
    }
}
