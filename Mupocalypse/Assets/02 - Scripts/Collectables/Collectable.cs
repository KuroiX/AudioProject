using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[CreateAssetMenu(menuName = "Collectable")]
public class Collectable : ScriptableObject
{
    public enum Effect {
        none,
        heal,
        heart,
        unlockAbility,
        key
    }

    public Sprite sprite;
    public Effect effect;

    [ConditionalHide("effect", 3)]
    public Player.Ability ability;
    [ConditionalHide("effect", 4)]
    public string id;

    public void ApplyEffect(Player player)
    {
        switch (effect)
        {
            case Effect.heal:
                player.Heal();
                break;
            case Effect.heart:
                player.IncreaseMaxHealth();
                break;
            case Effect.unlockAbility:
                player.UnlockAbility(ability);
                break;
        }
    }
}
