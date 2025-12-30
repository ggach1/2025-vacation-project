using Code.Entities;
using Code.System;
using UnityEngine;

namespace Code.Bocchi
{
    public class Player : Entity
    {
        [field : SerializeField] public InputSO InputSO { get; private set; }
    }
}