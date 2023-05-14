using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StoveCounter : BaseCounter
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs{
        public State state;
    }

    public enum State{
        Idle,
        Frying,
        Fried,
        Burned
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeArray;

    private State state;

    private float fryingTimer;
    private float burningTimer;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    private void Start() {
        state = State.Idle;
    }
    private void Update() {
        if(HasKitchenObject()){
            switch(state){
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    if(fryingTimer >= fryingRecipeSO.fryingTimerMax){
                        //Frying is done
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                        Debug.Log(burningRecipeSO);
                        state = State.Fried;
                        burningTimer = 0f;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{state = state});
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;
                    if(burningTimer >= burningRecipeSO.burningTimerMax){
                        //Frying is done
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        state = State.Burned;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{state = state});

                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }
    public override void Interact(Player player)
    {
        if(!HasKitchenObject()){
            //There is no kitchen object on the counter
            if(player.HasKitchenObject()){
                //Player is carrying something
                if(HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())){
                    //Player is carrying something that can be fried
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    fryingRecipeSO = GetFryingSORecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    state = State.Frying;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{state = state});

                    fryingTimer = 0f;

                }

            }else{
                //Player not carrying anything
            }
        }else{
            // There is a kitchen object on the counter
            if(player.HasKitchenObject()){
                //Player is carrying something
            }else{
                //Player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);

                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{state = state});

            }
        }
    }


    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO){
        FryingRecipeSO fryingRecipeSO = GetFryingSORecipeSOWithInput(inputKitchenObjectSO);

        return fryingRecipeSO != null;

  
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO){
        FryingRecipeSO fryingRecipeSO = GetFryingSORecipeSOWithInput(inputKitchenObjectSO);

        if(fryingRecipeSO != null){
            return fryingRecipeSO.output;
        }else{
            return null;
        }
  
    }

    private FryingRecipeSO GetFryingSORecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO){
        foreach(FryingRecipeSO fryingRecipeSO in fryingRecipeArray){
            if(fryingRecipeSO.input == inputKitchenObjectSO){
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO){
        foreach(BurningRecipeSO burningRecipeSO in burningRecipeArray){
            if(burningRecipeSO.input == inputKitchenObjectSO){
                return burningRecipeSO;
            }
        }
        return null;
    }
}
