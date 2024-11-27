using TMPro;
using UnityEngine;
public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text ammoCountText;
    [SerializeField] private TMP_Text magazineCountText;
    void OnEnable()
    {
        FPSController.OnAmmoCountChanged += UpdateAmmoText;
    }
    void OnDisable()
    {
        FPSController.OnAmmoCountChanged -= UpdateAmmoText;
    }
    private void UpdateAmmoText(int currentAmmoCount, int maxAmmoCount, int magazineCount)
    {
        ammoCountText.text = currentAmmoCount + "/" + maxAmmoCount;
        magazineCountText.text = "Magazine Count: " + magazineCount;
    }
}
