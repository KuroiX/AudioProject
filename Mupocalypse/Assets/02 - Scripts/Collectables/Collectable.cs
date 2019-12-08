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
    public Color color = Color.white;
    public Effect effect;

    [ConditionalHide("effect", 3)]
    public Player.Ability ability;
    [ConditionalHide("effect", 4)]
    public string id;
    public bool showMessage = true;
    [ConditionalHide("showMessage")]
    public string message;

    public void ApplyEffect(Player player)
    {
        switch (effect)
        {
            case Effect.heal:
                player.Heal();
                break;
            case Effect.heart:
                player.IncreaseMaxHealth();
                player.Heal();
                break;
            case Effect.unlockAbility:
                player.UnlockAbility(ability);
                break;
        }
    }
}
