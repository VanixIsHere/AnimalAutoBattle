using UnityEngine;

[CreateAssetMenu(fileName = "NewAssetCredit", menuName = "AssetCredit")]
public class AssetCredit : ScriptableObject {
    public string assetName;
    public string author;
    public string license;
    public string sourceURL;
}
