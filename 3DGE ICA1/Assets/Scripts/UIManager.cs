using TMPro;
using UnityEngine;
public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text ammoCountText;
    void OnEnable()
    {
        FPSController.OnAmmoCountChanged += UpdateAmmoText;
    }
    void OnDisable()
    {
        FPSController.OnAmmoCountChanged -= UpdateAmmoText;
    }
    private void UpdateAmmoText(int currentAmmoCount, int maxAmmoCount)
    {
        ammoCountText.text = currentAmmoCount + "/" + maxAmmoCount;
    }
}
