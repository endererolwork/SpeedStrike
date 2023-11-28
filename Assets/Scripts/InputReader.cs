using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Race
{
    [CreateAssetMenu( fileName  = "InputReader", menuName = "Race/InputReader" )]
    public class InputReader : ScriptableObject, PlayerInputActions.IPlayerActions
    {
        public Vector2 Move => inputActions.Player.Move.ReadValue<Vector2>();
        public bool IsBraking => inputActions.Player.Brake.ReadValue<float>() > 0;
        
        private PlayerInputActions inputActions;

        void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(instance:this);
                
            }
            
        }

        public void Enable()
        {
            inputActions.Enable(); 
        } 

        public void OnMove(InputAction.CallbackContext context)
        {
         
        }

        public void OnLook(InputAction.CallbackContext context)
        {
           
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            
        }

        public void OnBrake(InputAction.CallbackContext context)
        {
            
        }
    } 
    
}
    
