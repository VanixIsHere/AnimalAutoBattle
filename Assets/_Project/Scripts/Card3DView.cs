using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card3DView : MonoBehaviour
{
    [Header("Mesh References")]
    [SerializeField] private GameObject topFaceObject;
    [SerializeField] private GameObject bottomFaceObject;

    [Header("Text Elements")]
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private TextMeshPro specializationText;
    [SerializeField] private TextMeshPro costText;

    [Header("Class Related")]
    [SerializeField] private ClassIconLibrary classIconLibrary;
    [SerializeField] private Image classIconImage;

    private UnitData data;

    public void Init(UnitData unit)
    {
        data = unit;

        // FRONT ART
        if (unit.cardArtwork != null)
        {
            MeshRenderer frontRenderer = topFaceObject.GetComponent<MeshRenderer>();
            frontRenderer.material.SetTexture("_MainTex", unit.cardArtwork);
            // frontRenderer.material.SetTexture("_NoiseTexture", unit.cardArtwork);
            // frontRenderer.material.SetTexture("_MainTex", unit.cardArtwork);
            Debug.Log("Shader name: " + frontRenderer.material.shader.name);
        }

        // NAME
        if (nameText != null)
        {
            nameText.text = unit.unitName;
        }
        if (specializationText != null)
        {
            specializationText.text = unit.role.ToString();
        }
        if (costText != null)
        {
            costText.text = unit.cost.ToString();
        }
        if (classIconImage != null)
        {
            var icon = classIconLibrary.GetIcon(data.UnitClass);
            classIconImage.sprite = icon;
        }

        // BACK ART (optional)
        // MeshRenderer backRenderer = bottomFaceObject.GetComponent<MeshRenderer>();
        // backRenderer.material.SetTexture("_MainTex", unit.cardBackTexture);
    }
}
