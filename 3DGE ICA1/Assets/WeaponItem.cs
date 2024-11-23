using UnityEngine;
public class WeaponItem : MonoBehaviour, Item
{
    //[SerializeField] private int magazineAmount;
    public void Use(FPSController playerController)
    {
        transform.SetParent(playerController.WeaponHolder.transform);
        gameObject.layer = 6;
        foreach (Transform child in transform)
        {
            child.gameObject.layer = 6;
        }
        //currentWeapon.transform.parent = TheWorld.transform;
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        gameObject.GetComponent<IKWeaponGrab>().enabled = true;
        Weapon weapon = gameObject.GetComponent<Weapon>();
        weapon.enabled = true;

        for (int i = 0; i < playerController.weapons.Length; i++)
        {
            if (playerController.weapons[i] == null)
            {
                playerController.weapons[i] = weapon;
                break;
            }
        }
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        gameObject.SetActive(false);
    }
}
