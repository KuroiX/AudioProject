﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collectable")]
public class Collectable : ScriptableObject
{
    public enum Effect {
        heal
    }

    public Effect effect;
    public Sprite sprite;

    public void ApplyEffect(Player player)
    {
        switch (effect)
        {
            case Effect.heal:
                player.Heal();
                break;
        }
    }
}
