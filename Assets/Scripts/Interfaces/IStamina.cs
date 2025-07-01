using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStamina
{
    public float CurrentStamina { get; set; }
    public float MaxStamina { get; set; }
    public float DrainScaler {  get; set; }
    public float ReplenishScaler { get; set; }
    public Action<float> OnStaminaChanged { get; set; }

    public void DrainStamina();
    public void ReplenishStamina();
    public void SetStamina(float value);
}
