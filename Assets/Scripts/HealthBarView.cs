using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour
{
    public List<Image> heartImages = new List<Image>();

    public void SetHealth(int health) {
        if (health > heartImages.Count) {
            Debug.LogWarning("Attempting to set health view to more hearts than available!");
            health = heartImages.Count;
        } else if (health < 0) {
            Debug.LogWarning("Attempting to set health view to a negative value!");
            health = 0;
        }

        for (int i = 0; i < heartImages.Count; i++) {
            heartImages[i].enabled = (i < health);
        }
    }
}
