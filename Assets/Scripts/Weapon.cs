using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{

    [SerializeField] private AudioSource semiAutoSound;
    [SerializeField] private AudioSource fullyAutoSound;
    [SerializeField] private AudioSource hitSound;
    [SerializeField] private AudioSource pickUpAmmo;

    [SerializeField] private float fullAutoBloomAdjustment;
    [SerializeField] private float shotgunBloomAdjustment;

    [SerializeField] private GameObject bulletHolePrefab;

    [SerializeField] private Image hitMarkerImage;

    [SerializeField] private Image[] weaponPreviewImages = new Image[6];

    [SerializeField] private int numberOfClips;

    [SerializeField] private LayerMask canBeShot;

    [SerializeField] private Text ammoText;
    [SerializeField] private Text ammoPickupText;

    [SerializeField] private Transform parent;

    private AudioSource gunAudio;

    private bool isAiming = false;
    private bool reloading = false;
    private bool isMelee;
    private bool isFullAuto;
    private bool isShotgun;
    private bool ammoEmpty = false;
    private bool hitMarkerVisible;
    private bool ammoPickedUp;

    private bool[] weaponHasBeenEquipped;

    private Color hitMarkerTransparent;
    private Color hitMarkerOpaque;
    private Color slotColor;

    private Coroutine reloadCo;
    private Coroutine hitMarkerCo;

    private Dictionary<int, Tuple<int, int>> weaponAmmoCount = new Dictionary<int, Tuple<int, int>>();

    private float currentCooldown;
    private float timeSinceLastShot;
    private float currentBloom;
    private float currentRecoil;
    private float currentKickback;
    private float currentFireRate;
    private float weaponBloom;
    private float weaponRecoil;
    private float weaponKickback;
    private float weaponFireRate;
    private float currentFullAutoBloom;
    private float hitMarkerWait;
    private float ammoPickupTime;

    private GameObject currentWeapon;
    private GameObject player;

    private int activeWeaponSlot = 0;
    private int aimSpeed = 10;
    private int weaponDamage;
    private int magazineCapacity;
    private int maxAmmo;
    private int currentWeaponType;
    private int numWeaponsEquipped;
    private int loadoutSize = 6;

    private int[] ammoInMagazine;
    private int[] weaponMaxAmmo = new int[5] { 0, 84, 120, 24, 20 };

    private MeleeWeapon meleeComponnent;

    private Transform startPos;

    private WeaponObject[] loadout;

    // Start is called before the first frame update
    void Start()
    {
        loadout = new WeaponObject[loadoutSize];
        ammoInMagazine = new int[loadout.Length];
        weaponHasBeenEquipped = new bool[loadout.Length];
        player = GameObject.FindWithTag("Player");
        hitMarkerOpaque = new Color(1, 1, 1, 1);
        slotColor = weaponPreviewImages[0].color;
        slotColor.a = 1;
    }

    // Update is called once per frame
    void Update()
    {

        if(!Camera.main.GetComponent<Look>().isPaused())
        {
            timeSinceLastShot += Time.deltaTime;

            if (ammoPickedUp)
            {
                if (ammoPickupTime < 1)
                {
                    ammoPickupTime += Time.deltaTime;
                }
                else
                {
                    Color temp = ammoPickupText.color;
                    temp.a -= Time.deltaTime;
                    ammoPickupText.color = temp;

                    if (ammoPickupText.color.a == 0)
                    {
                        ammoPickedUp = false;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetWeaponSlotActive(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetWeaponSlotActive(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetWeaponSlotActive(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetWeaponSlotActive(3);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SetWeaponSlotActive(4);
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                SetWeaponSlotActive(5);
            }

            if (hitMarkerVisible)
            {
                if (hitMarkerWait > 0)
                {
                    hitMarkerWait -= Time.deltaTime;
                }
                else
                {
                    hitMarkerImage.CrossFadeAlpha(0, 0.5f, false);
                    hitMarkerVisible = false;
                }
            }



            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                int newIndex = activeWeaponSlot + 1;
                if (newIndex >= loadout.Length)
                    newIndex = 0;

                SetWeaponSlotActive(newIndex);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                int newIndex = activeWeaponSlot - 1;
                if (newIndex < 0)
                    newIndex = loadout.Length - 1;

                SetWeaponSlotActive(newIndex);
            }

            if (isMelee)
            {
                if (currentCooldown <= 0f && Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(Melee());
                }

                if (currentWeapon != null && (currentWeapon.transform.localPosition != Vector3.zero))
                {
                    currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);

                    if (currentWeapon.transform.Find("Anchor").rotation != player.transform.Find("Main Camera/Weapon").rotation)
                    {
                        currentWeapon.transform.Find("Anchor").rotation = Quaternion.Lerp(currentWeapon.transform.Find("Anchor").rotation, player.transform.Find("Main Camera/Weapon").rotation * Quaternion.identity, Time.deltaTime * 4);
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.R) && loadout[activeWeaponSlot] != null && !reloading && (ammoInMagazine[activeWeaponSlot] < magazineCapacity) && (weaponAmmoCount[currentWeaponType].Item1 > 0))
                {
                    reloadCo = StartCoroutine(Reload());
                }

                if (currentWeapon != null)
                {
                    Aim(Input.GetMouseButton(1));

                    if (!reloading)
                    {
                        if (isFullAuto)
                        {
                            if (ammoEmpty)
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    if (weaponAmmoCount[currentWeaponType].Item1 > 0)
                                    {
                                        reloadCo = StartCoroutine(Reload());
                                    }
                                    else
                                    {
                                        gunAudio.PlayOneShot(gunAudio.clip);
                                    }
                                }
                            }
                            else
                            {
                                if (currentCooldown <= 0f && Input.GetMouseButton(0))
                                {
                                    currentRecoil = Mathf.Min(loadout[activeWeaponSlot].maxRecoilBloomAmount, currentRecoil * loadout[activeWeaponSlot].recoilBloomIncreaseRate);
                                    gunAudio.PlayOneShot(gunAudio.clip);
                                    Shoot();
                                }
                                if (Input.GetMouseButtonUp(0))
                                {
                                    currentRecoil = weaponRecoil / loadout[activeWeaponSlot].recoilBloomFactor;
                                }
                            }

                        }
                        else
                        {
                            if (currentCooldown <= 0f && Input.GetMouseButtonDown(0))
                            {
                                gunAudio.PlayOneShot(gunAudio.clip);
                                Shoot();
                            }
                        }
                    }
                    currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
                }
            }

            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }
            else
            {
                currentCooldown = 0;
            }
        }

    }

    private void Aim(bool RMB)
    {
        Transform anchor = currentWeapon.transform.Find("Anchor");
        Transform hip = currentWeapon.transform.Find("States/Hip");
        Transform ads = currentWeapon.transform.Find("States/ADS");

        if (RMB && !reloading)
        {
            anchor.position = Vector3.Lerp(anchor.position, ads.position, Time.deltaTime * aimSpeed);
            isAiming = true;
            if (!isFullAuto)
                currentFireRate = weaponFireRate * 2;
            if (!isShotgun)
                currentBloom = weaponBloom / 3;
        }
        else
        {
            anchor.position = Vector3.Lerp(anchor.position, hip.position, Time.deltaTime * aimSpeed);
            isAiming = false;
            currentFireRate = weaponFireRate;
            currentBloom = weaponBloom;
        }
    }

    IEnumerator Equip(int index)
    {

        yield return new WaitForSeconds(.25f);


        if (currentWeapon != null)
            Destroy(currentWeapon);

        GameObject newEquip = Instantiate(loadout[index].prefab, parent.position, parent.rotation, parent);
        isMelee = loadout[index].melee;
        newEquip.transform.localPosition = Vector3.zero;
        newEquip.transform.localEulerAngles = Vector3.zero;

        currentWeapon = newEquip;
        if (isMelee)
        {
            meleeComponnent = currentWeapon.GetComponent<MeleeWeapon>();
        }
        currentWeaponType = loadout[index].type;

        startPos = currentWeapon.transform.Find("Anchor");
        weaponBloom = loadout[index].bloom;
        weaponRecoil = loadout[index].recoil;
        currentRecoil = weaponRecoil / loadout[index].recoilBloomFactor;
        weaponKickback = loadout[index].kickback;
        weaponFireRate = loadout[index].fireRate;
        weaponDamage = loadout[index].damage;
        isFullAuto = loadout[index].fullAuto;


        if (isFullAuto)
            gunAudio = fullyAutoSound;
        else
            gunAudio = semiAutoSound;

        gunAudio.clip = loadout[index].shot;

        isShotgun = loadout[index].shotgun;

        if (isFullAuto || isShotgun)
            currentFullAutoBloom = fullAutoBloomAdjustment;
        else
            currentFullAutoBloom = 0f;

        magazineCapacity = loadout[index].magazineSize;

        if (!weaponHasBeenEquipped[index])
        {
            weaponHasBeenEquipped[index] = true;
            if (!weaponAmmoCount.ContainsKey(currentWeaponType))
            {
                SetAmmo(currentWeaponType);
            }
            ammoInMagazine[index] = 0;
            SetAmmoText();
            if (weaponAmmoCount[currentWeaponType].Item2 > 0)
            {
                reloadCo = StartCoroutine(Reload());
            }
            else
            {
                gunAudio.clip = loadout[index].emptyFire;
                ammoEmpty = true;
            }
        }
        else
        {
            if (ammoInMagazine[index] > 0)
            {
                gunAudio.clip = loadout[index].shot;
                ammoEmpty = false;
            }
            else
            {
                gunAudio.clip = loadout[index].emptyFire;
                ammoEmpty = true;
            }
            SetAmmoText();
        }

    }

    private void Shoot()
    {
        if (ammoInMagazine[activeWeaponSlot] > 0)
        {
            //Transform spawn = currentWeapon.transform.Find("Anchor/Design/Chamber Exit");
            Transform spawn = transform.Find("Main Camera");
            Vector3 relativeVelocity = Quaternion.Inverse(player.transform.rotation) * player.GetComponent<CharacterController>().velocity;

            int numOfShots = 1;
            bool enemyHit = false;
            float playerSpeed = Mathf.Round(relativeVelocity.z * 10f) / 10f;
            float speedCorrection = Mathf.Max(1f, Mathf.Abs(playerSpeed) / 4f);
            float timeMultiplier = 5f;
            float numerator = currentBloom * speedCorrection;
            float denominator;

            if (isFullAuto)
            {
                denominator = timeSinceLastShot * (timeMultiplier + fullAutoBloomAdjustment);
            }
            else if (isShotgun)
            {
                numerator /= speedCorrection;
                denominator = shotgunBloomAdjustment;
                numOfShots = 20;
            }
            else
            {
                denominator = timeSinceLastShot * timeMultiplier;
            }

            float adjustedBloom = numerator / denominator;

            for (int i = 0; i < numOfShots; i++)
            {
                Vector3 newBloom = spawn.position + spawn.forward * 1000f;
                newBloom += Random.Range(-adjustedBloom - currentRecoil, adjustedBloom + currentRecoil) * spawn.up;
                newBloom += Random.Range(-adjustedBloom - currentRecoil, adjustedBloom + currentRecoil) * spawn.right;
                newBloom -= spawn.position;
                newBloom.Normalize();
                RaycastHit hit;
                if (Physics.Raycast(spawn.position, newBloom, out hit, 1000f, canBeShot))
                {
                    //Debug.Log(hit.transform.gameObject.name);
                    if (hit.transform.gameObject.CompareTag("Enemy"))
                    {
                        enemyHit = true;
                        hit.transform.gameObject.GetComponent<Health>().TakeDamage(weaponDamage / numOfShots);
                    }
                    else if (!hit.transform.gameObject.CompareTag("Projectile"))
                    {
                        GameObject newHole = Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.001f, Quaternion.identity);
                        newHole.transform.LookAt(hit.point + hit.normal);
                        newHole.transform.Rotate(new Vector3(90f, 0, 0));
                        Destroy(newHole, 3f);
                    }
                }
            }

            if (enemyHit)
            {
                if (hitMarkerCo != null)
                {
                    StopCoroutine(hitMarkerCo);
                }

                hitMarkerCo = StartCoroutine(playSoundAfterTime(hitSound, .005f));

            }

            currentWeapon.transform.Rotate(-(weaponRecoil + currentRecoil / loadout[activeWeaponSlot].recoilRotateFactor), 0, 0);
            currentWeapon.transform.position -= currentWeapon.transform.forward * weaponKickback;

            currentCooldown = currentFireRate;

            timeSinceLastShot = 0;

            ammoInMagazine[activeWeaponSlot]--;

            SetAmmoText();

            if (ammoInMagazine[activeWeaponSlot] <= 0)
            {
                ammoEmpty = true;
                gunAudio.clip = loadout[activeWeaponSlot].emptyFire;

                //make method?
                if (weaponAmmoCount[currentWeaponType].Item1 > 0)
                    reloadCo = StartCoroutine(Reload());
            }
        }
        else
        {
            //make method?
            if (weaponAmmoCount[currentWeaponType].Item1 > 0)
                reloadCo = StartCoroutine(Reload());
        }

    }

    IEnumerator Melee()
    {
        if (currentWeapon != null)
        {
            meleeComponnent.SetAttack(weaponDamage, true);
            currentWeapon.transform.position -= currentWeapon.transform.forward * weaponKickback;
            currentWeapon.transform.Find("Anchor").Rotate(-weaponRecoil, -45, 0);
            currentCooldown = weaponFireRate;
            yield return new WaitForSeconds(weaponFireRate / 4);
            meleeComponnent.SetAttack(0, false);
        }

    }

    IEnumerator Reload()
    {

        int currentAmmo = weaponAmmoCount[currentWeaponType].Item1;
        int currentMaxAmmo = weaponAmmoCount[currentWeaponType].Item2;
        int difference = magazineCapacity - ammoInMagazine[activeWeaponSlot];

        reloading = true;

        yield return new WaitForSeconds(2);

        currentRecoil = weaponRecoil / 100f;

        if (currentAmmo > difference)
        {
            currentAmmo -= difference;
            ammoInMagazine[activeWeaponSlot] = magazineCapacity;
        }
        else
        {
            ammoInMagazine[activeWeaponSlot] += currentAmmo;
            currentAmmo = 0;
        }

        weaponAmmoCount[currentWeaponType] = new Tuple<int, int>(currentAmmo, currentMaxAmmo);

        if (GameObject.FindGameObjectWithTag("TestScene") != null)
        {
            if (GameObject.FindGameObjectWithTag("TestScene").gameObject.GetComponent<TestScene>().testScene == true)
            {
                weaponAmmoCount[currentWeaponType] = new Tuple<int, int>(9999, 9999);
            }
        }

        gunAudio.clip = loadout[activeWeaponSlot].shot;
        SetAmmoText();
        reloading = false;
        ammoEmpty = false;
    }

    public bool GetIsAiming()
    {
        return isAiming;
    }

    void SetAmmoText()
    {
        ammoText.text = (ammoInMagazine[activeWeaponSlot] + " / " + magazineCapacity + "  " + weaponAmmoCount[currentWeaponType].Item1);
    }

    public bool PickupAmmo(int type, int amount)
    {

        if (!weaponAmmoCount.ContainsKey(type))
        {
            SetAmmo(type);
        }

        int currentAmmoInMagazine = ammoInMagazine[currentWeaponType];
        int maxAmmoOfType = weaponAmmoCount[type].Item2;

        if (currentAmmoInMagazine + weaponAmmoCount[type].Item1 >= maxAmmoOfType)
            return false;

        int newAmount = weaponAmmoCount[type].Item1 + amount;

        if ((currentAmmoInMagazine + newAmount) >= maxAmmoOfType)
        {
            newAmount = maxAmmoOfType - currentAmmoInMagazine;
        }

        string typeName = "";

        switch (type)
        {
            case 1:
                typeName = "9mm";
                break;
            case 2:
                typeName = "AR 5.56";
                break;
            case 3:
                typeName = "12-Gauge";
                break;
        }

        ammoPickupTime = 0;
        ammoPickupText.color = slotColor;
        ammoPickupText.text = "+" + (newAmount - weaponAmmoCount[type].Item1) + " " + typeName;
        ammoPickedUp = true;

        weaponAmmoCount[type] = new Tuple<int, int>(newAmount, maxAmmoOfType);

        SetAmmoText();

        pickUpAmmo.Play();
        return true;
    }

    private void SetAmmo(int type)
    {
        if (type > 0)
        {
            weaponAmmoCount[type] = new Tuple<int, int>(0, weaponMaxAmmo[type]);

            if (GameObject.FindGameObjectWithTag("TestScene") != null)
            {
                if (GameObject.FindGameObjectWithTag("TestScene").gameObject.GetComponent<TestScene>().testScene == true)
                {
                    weaponAmmoCount[type] = new Tuple<int, int>(9999, 9999);
                }
            }
        }
        else
        {
            weaponAmmoCount[type] = new Tuple<int, int>(0, 0);
        }
    }

    IEnumerator playSoundAfterTime(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        hitMarkerVisible = true;
        hitMarkerWait = .5f;
        hitMarkerImage.color = hitMarkerOpaque;
        hitMarkerImage.CrossFadeAlpha(1, 0.001f, false);
        source.Play();
    }

    public void PickUpWeapon(WeaponObject newWeapon)
    {
        int currIndex = 0;

        if (numWeaponsEquipped < loadoutSize)
        {
            if (loadout[activeWeaponSlot] == null)
            {
                loadout[activeWeaponSlot] = newWeapon;
                weaponPreviewImages[activeWeaponSlot].color = slotColor;
                weaponPreviewImages[activeWeaponSlot].sprite = newWeapon.previewImage;
                StartCoroutine(Equip(activeWeaponSlot));
            }
            else
            {
                while (loadout[currIndex] != null)
                {
                    if (currIndex == loadoutSize - 1)
                        currIndex = 0;
                    else
                        currIndex++;
                }
                loadout[currIndex] = newWeapon;
                weaponPreviewImages[currIndex].color = slotColor;
                weaponPreviewImages[currIndex].sprite = newWeapon.previewImage;
            }
            numWeaponsEquipped++;
        }
        else
        {
            loadout[activeWeaponSlot] = newWeapon;
            weaponPreviewImages[activeWeaponSlot].color = slotColor;
            weaponPreviewImages[activeWeaponSlot].sprite = newWeapon.previewImage;
            StartCoroutine(Equip(activeWeaponSlot));
        }

    }

    private void SetWeaponSlotActive(int newActiveSlot)
    {
        Color tempSlotColour = slotColor;

        tempSlotColour.a = 0;
        weaponPreviewImages[activeWeaponSlot].transform.Find("Outline").GetComponent<Image>().color = tempSlotColour;

        activeWeaponSlot = newActiveSlot;
        weaponPreviewImages[activeWeaponSlot].transform.Find("Outline").GetComponent<Image>().color = slotColor;

        if (reloadCo != null)
        {
            StopCoroutine(reloadCo);
            reloading = false;
        }

        if (loadout[newActiveSlot] != null)
        {
            StartCoroutine(Equip(newActiveSlot));
        }
        else
        {
            Destroy(currentWeapon);
        }
    }
}