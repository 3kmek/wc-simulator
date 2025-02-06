using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoldable
{
    void OnPickup(Transform holdPosition);
    void OnDrop();
    bool IsHolding { get; }
    void Use();
}
