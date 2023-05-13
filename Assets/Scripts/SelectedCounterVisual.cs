using UnityEngine;
using System;

public class SelectedCounterVisual: MonoBehaviour
{

    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] gameObjectsArray;

    private void  Start()
    {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if(e.selectedCounter == baseCounter){
           Show();
        }else{
            Hide();
        }
    }

    private void Show(){
        foreach(GameObject visualGameObject in gameObjectsArray){
            visualGameObject.SetActive(true);
        }
    }

    private void Hide(){
        foreach(GameObject visualGameObject in gameObjectsArray){
            visualGameObject.SetActive(false);
        }
    }
}