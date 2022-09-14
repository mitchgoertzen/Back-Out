using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Consumable")]
public class ConsumableItem : ScriptableObject
{

    public Sprite previewImage;

    public new string name;

    public int type;

    public int size;

}
