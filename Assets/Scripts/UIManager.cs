using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    public Sprite[] documentIcons;

    public Image[] inventoryImgs;

    private Player player;


    private void Start()
    {
        player = (Player)FindAnyObjectByType(typeof(Player));
    }


    public Sprite getMatchingIcon(documentColor color, documentType type)
    {
        int colorIndex = 0;
        int shapeIncrement = 0;

        switch (color)
        {
            case documentColor.red:
                colorIndex = 0;
                break;
            case documentColor.blue:
                colorIndex = 4;
                break;
            case documentColor.yellow:
                colorIndex = 8;
                break;
            case documentColor.pink:
                colorIndex = 12;
                break;
            case documentColor.purple:
                colorIndex = 16;
                break;
        }

        switch (type)
        {
            case (documentType.star):
                shapeIncrement = 0;
                break;
            case (documentType.circle):
                shapeIncrement = 1;
                break;
            case (documentType.triangle):
                shapeIncrement = 2;
                break;
            case (documentType.square):
                shapeIncrement = 3;
                break;

        }

        return documentIcons[colorIndex + shapeIncrement];
    }

    public void refreshInventoryUI()
    {
        Pickup[] playerInv = player.getInventory();

        for (int i = 0; i < playerInv.Length; i++)
        {
            if (playerInv[i])
                inventoryImgs[i].sprite = playerInv[i].icon;
            else
                inventoryImgs[i].sprite = null;
        }
    }
}
