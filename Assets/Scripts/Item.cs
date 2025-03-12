using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type
    {
        Empty,
        AetherCrystal,
        EnvoyRune,
        FlameVial,
        Key,
        NauticShard,
        Stone,
        Shield,
        Hope,
        FishEgg,
        RecallStone,
        Flower,
        Sword,
        Glyph
    }
    public Type itemType;

    [Tooltip("Item Effect Value, for example 10 Heal")]
    public int value = 1;

    //public Item(Type itemType)
    //{
    //    this.itemType = itemType;
    //}

    public Type GetItemType()
    {
        return itemType;
    }
}
