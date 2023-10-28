using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UFOAttack : MonoBehaviour
{

    [SerializeField] private float _fireCooldownTime = 0.3f;
    [SerializeField] private UnityEvent OnShoot;
   
    
    private void OnEnable()
    {
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        while (enabled)
        {
            OnShoot?.Invoke();
            yield return new WaitForSeconds(_fireCooldownTime);
        }
    }
}
