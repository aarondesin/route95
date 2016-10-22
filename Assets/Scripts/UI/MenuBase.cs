// MenuBase.cs
// ©2016 Team 95

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public abstract class MenuBase<T> : SingletonMonoBehaviour<T> 
    where T : MonoBehaviour 
{


    public void Show () {

    }

    public void Hide () {

    }

}
