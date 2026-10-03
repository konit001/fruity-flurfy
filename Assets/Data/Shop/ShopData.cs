using UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "Shop/ShopData")]
public class ShopData : ScriptableObject
{
    public string id;
    public string _Name;
    public int _Price;
    public Sprite _Icon;
    [TextArea(15, 20)]
    public string _Description;
}
