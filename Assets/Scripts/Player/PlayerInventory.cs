using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<GameObject> Weapons = new List<GameObject>();

    GameManager gm;
    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        foreach (GameObject weapon in gm.Weapons)
        {
            Weapons.Add(weapon);
        }
    }

    public GameObject NextWeapon(GameObject current)
    {
        //Weapons[Weapons.IndexOf(current)].GetComponent<GunScript>().currentAmmo = current.GetComponent<GunScript>().currentAmmo;
        if (Weapons.Count == 0)
        {
            return null;
        }
        else
        {
            int index = Weapons.IndexOf(current) + 1;
            if (index > Weapons.Count - 1)
            {
                return Weapons[0];
            }
            else
            {
                return Weapons[1];
            }
        }
        
    }

    public GameObject PreviousWeapon(GameObject current)
    {
        //Weapons[Weapons.IndexOf(current)].GetComponent<GunScript>().currentAmmo = current.GetComponent<GunScript>().currentAmmo;
        if (Weapons.Count == 0)
        {
            return null;
        }
        else
        {
            int index = Weapons.IndexOf(current) - 1;
            if (index < 0)
            {
                return Weapons[Weapons.Count - 1];
            }
            else
            {
                return Weapons[index];
            }
        }
    }
}
