using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    [SerializeField] private AudioSource pickUpHealth;
    [SerializeField] private AudioSource useHealth;

    [SerializeField] private Image itemPreviewImage;

    [SerializeField] private Text itemCount;

    private Color visible;
    private Color invisible;

    private Dictionary<string, List<ConsumableItem>> inventory = new Dictionary<string, List<ConsumableItem>>();

    private int currentIndex = 0;

    private void Start()
    {
        visible = itemPreviewImage.color;
        visible.a = 1;
        invisible = itemPreviewImage.color;
        invisible.a = 0;
    }

    void Update()
    {
        if (inventory.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Use();
            }
        }

        if (inventory.Count > 1)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                CycleItems(true);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                CycleItems(false);
            }
        }

    }

    private void Use()
    {
        useHealth.Play();

        List<ConsumableItem> tempList = inventory.ElementAt(currentIndex).Value;

        ConsumableItem tempItem = tempList[tempList.Count - 1];

        if (tempItem != null)
        {
            switch (tempItem.type)
            {
                case 0:
                    {
                        GetComponentInParent<PlayerAttributes>().AddHealth(tempItem.size);
                    }
                    break;
            }
        }


        inventory.ElementAt(currentIndex).Value.RemoveAt(tempList.Count - 1);

        //removing item list
        if (inventory.ElementAt(currentIndex).Value.Count == 0)
        {
            inventory.Remove(inventory.ElementAt(currentIndex).Key);

            if(inventory.Count == 0)
            {
                itemPreviewImage.color = invisible;
                itemCount.color = invisible;
            }
            else
            {
                if(currentIndex == inventory.Count)
                {
                    currentIndex--;
                }

                itemPreviewImage.sprite = inventory.ElementAt(currentIndex).Value[0].previewImage;
                itemCount.text = inventory.ElementAt(currentIndex).Value.Count.ToString();
            }
        }
        else
        {

            itemCount.text = inventory.ElementAt(currentIndex).Value.Count.ToString();
        }
    }

    private void CycleItems(bool forward)
    {
        if (forward)
        {
            if (currentIndex == inventory.Count - 1)
                currentIndex = 0;
            else
                currentIndex++;
        }
        else
        {
            if (currentIndex == 0)
                currentIndex = inventory.Count - 1;
            else
                currentIndex--;
        }

        itemPreviewImage.sprite = inventory.ElementAt(currentIndex).Value[0].previewImage;
        itemCount.text = inventory.ElementAt(currentIndex).Value.Count.ToString();

    }

    public bool PickUp(ConsumableItem newItem)
    {
        if (!inventory.ContainsKey(newItem.name))
        {
            if(inventory.Count == 0)
            {
                itemPreviewImage.sprite = newItem.previewImage;
                itemPreviewImage.color = visible;
                itemCount.color = visible;
            }

            List <ConsumableItem> newList = new List<ConsumableItem> { newItem };
            inventory.Add(newItem.name, newList);
            
        }
        else
        {
            inventory[newItem.name].Add(newItem);
        }

        itemCount.text = inventory.ElementAt(currentIndex).Value.Count.ToString();

        pickUpHealth.Play();

        return true;
    }
}
