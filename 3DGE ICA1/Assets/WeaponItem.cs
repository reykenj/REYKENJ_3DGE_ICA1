using UnityEngine;
public class WeaponItem : MonoBehaviour, Item
{
    //[SerializeField] private int magazineAmount;
    [SerializeField] private AudioClip PickUpSFX;
    public void Use(FPSController playerController)
    {
        if (PickUpSFX != null)
        {
            AudioSource.PlayClipAtPoint(PickUpSFX, transform.position);
        }
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
        bool Equipped = false;
        weapon.DropUI.SetActive(false);
        //playerController.RightArm.SetActive(true);
        for (int i = 0; i < playerController.weapons.Length; i++)
        {
            if (playerController.weapons[i] == null)
            {
                if (!Equipped)
                {
                    Equipped = true;
                    playerController.weapons[i] = weapon;
                    break;
                }
            }
        }
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        gameObject.SetActive(false);
    }
}
