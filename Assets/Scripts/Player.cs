using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance {get; private set;}

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs{
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    private bool isWalking;
    private Vector3 lastInteractDir;

    private BaseCounter selectedCounter;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private KitchenObject kitchenObject;



    private void Awake()
    {
        if(Instance != null){
            Debug.LogError("More than one Player in scene!");
        }
        Instance = this;
    }

    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;    
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if(selectedCounter != null){
            selectedCounter.Interact(this);
        }
    }

    private void Update() {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking(){
        return isWalking;
    }

    private void HandleInteractions(){
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        float interactDistance = 2f;

        if(moveDir != Vector3.zero){
            lastInteractDir = moveDir;
        }

        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit hit, interactDistance,countersLayerMask)) {
           if(hit.collider.TryGetComponent(out BaseCounter baseCounter)){
            if(baseCounter != selectedCounter){
                selectedCounter = baseCounter;

                SetSelectedCounter(baseCounter);
            }
           }else {
                SetSelectedCounter(null);
           }
        }else{
           SetSelectedCounter(null);
        }

    }

    private void HandleMovement(){
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        float playerRadius = 0.7f;
        float playerHeight = 2f;
        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if(!canMove){
            //Cannot move towards moveDir

            //Attempt to X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if(canMove){
                moveDir = moveDirX;
            }else{
                //Attempt to Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if(canMove){
                    moveDir = moveDirZ;
                }
            }

        }
        if(canMove){
            transform.position += moveDir  * Time.deltaTime * moveSpeed;
        } 

        isWalking = moveDir != Vector3.zero;


        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter){
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs{selectedCounter = this.selectedCounter});
    }

    public Transform GetKitchenFollowTransform(){
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject){
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject(){
        return kitchenObject;
    }

    public void ClearKitchenObject(){
        kitchenObject = null;
    }

    public bool HasKitchenObject(){
        return kitchenObject != null;
    }
}
