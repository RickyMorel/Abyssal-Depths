using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using static SaveData;
using UnityEngine.Rendering;

public static class SaveUtils
{
    private static UpgradeChip[] _allChips = Resources.LoadAll<UpgradeChip>("ScriptableObjs/Chips");

    //public SaveUtils()
    //{
    //    _allChips = Resources.LoadAll<UpgradeChip>("ScriptableObjs/Chips");
    //}

    public static UpgradeChip GetChipById(string chipId)
    {
        foreach (UpgradeChip chip in _allChips)
        {
            if (chipId != chip.Id) { continue; }

            return chip;
        }

        return null;
    }

    public static Sprite ConvertBytesToTexture2D(byte[] byteArray)
    {
        Texture2D imageTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        imageTexture.LoadImage(byteArray);
        return Sprite.Create(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), Vector2.zero);
    }
}
