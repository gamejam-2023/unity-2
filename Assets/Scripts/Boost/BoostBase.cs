using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class BoostBase : MonoBehaviour
{
    public abstract void Apply(PlayerController player);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        if (other.TryGetComponent(out PlayerController player) == false)
        {
            return;
        }

        Apply(player);
        Destroy(gameObject);
    }
}
