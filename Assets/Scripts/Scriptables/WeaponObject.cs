using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponObject : ScriptableObject
{
    public GameObject prefab;

    public Sprite previewImage;

    public AudioClip shot;
    public AudioClip reload;
    public AudioClip emptyFire;

    public new string name;

    public int type;

    public int damage;
    public int magazineSize;

    public float shotVolume;
    public float fireRate;
    public float bloom;
    public float recoil;
    public float kickback;
    public float recoilBloomFactor;
    public float recoilRotateFactor;
    public float recoilBloomIncreaseRate;
    public float maxRecoilBloomAmount;

    public bool melee;
    public bool fullAuto;
    public bool shotgun;
}
