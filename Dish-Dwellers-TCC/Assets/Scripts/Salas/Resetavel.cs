using UnityEngine;
using UnityEngine.Events;

public class Resetavel : IResetavel{

    public UnityEvent onReset;

    
    public override void OnReset() => onReset?.Invoke();

}
