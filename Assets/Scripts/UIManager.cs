using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI score;

    [SerializeField]
    public Sprite[] documentIcons;
    public Sprite[] documentIconsCompleted;

    public Image[] inventoryImgs;

    public GameObject gameEndPanel;
    public TMPro.TextMeshProUGUI ordersValueTxt;
    public TMPro.TextMeshProUGUI tipsValueTxt;

    private Player player;


    private void Start()
    {
        player = (Player)FindAnyObjectByType(typeof(Player));

        gameEndPanel.transform.localScale = Vector3.zero;
        gameEndPanel.gameObject.SetActive(false);
    }


    public Sprite getMatchingIcon(documentColor color, documentType type, bool completed = false)
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

        if (completed)
            return documentIconsCompleted[colorIndex + shapeIncrement];
        else
            return documentIcons[colorIndex + shapeIncrement];
    }



    public void refreshInventoryUI()
    {
        Pickup[] playerInv = player.getInventory();

        for (int i = 0; i < playerInv.Length; i++)
        {
            if (playerInv[i])
            {
                inventoryImgs[i].sprite = playerInv[i].icon;
                Color newColor = inventoryImgs[i].color;
                newColor.a = 1; //why doesn't it just let you modify the color directly? who knows
                inventoryImgs[i].color = newColor;
            }
            else
            {
                inventoryImgs[i].sprite = null;
                Color newColor = inventoryImgs[i].color;
                newColor.a = 0;
                inventoryImgs[i].color = newColor;
            }
        }

        for (int i = 0; i < inventoryImgs.Length; i++)
        {
            if (i == player.selectedPickup)
            {
                inventoryImgs[i].transform.parent.DOScale(new Vector3(1.3f, 1.3f, 1.3f), .5f);
            }
            else
            {
                inventoryImgs[i].transform.parent.DOScale(new Vector3(1, 1, 1), .5f);
            }
        }
    }

    public void refreshScoreUI()
    {
        score.text = player.points.ToString();
    }

    public void endLevelUI(int ordersCompleted)
    {
        gameEndPanel.SetActive(true);
        ordersValueTxt.text = ordersCompleted.ToString();
        tipsValueTxt.text = player.points.ToString();

        gameEndPanel.transform.DOScale(Vector3.one, .5f);
    }
}
