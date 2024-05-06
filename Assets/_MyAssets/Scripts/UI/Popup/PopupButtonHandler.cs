using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupButtonHandler : MonoBehaviour
{
    public void OnPositiveButtonClick()
    {
         PopupHandler.Instance.SetButtonState(true);  
    }
    
    public void OnNegativeButtonClick()
    {
        PopupHandler.Instance.SetButtonState(false);
    }
}
