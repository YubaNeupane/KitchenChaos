using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO cutKitchenObjectSO;
    public override void Interact(Player player)
    {
        if(!HasKitchenObject()){
            //There is no kitchen object on the counter
            if(player.HasKitchenObject()){
                //Player is carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }else{
                //Player carrying anything
            }
        }else{
            // There is a kitchen object on the counter
            if(player.HasKitchenObject()){
                //Player is carrying something
            }else{
                //Player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if(HasKitchenObject()){
            //there is a kitchen object on the counter
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(cutKitchenObjectSO, this);
        }
    }
}
