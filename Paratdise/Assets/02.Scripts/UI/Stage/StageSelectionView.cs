using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ۼ��� : ������
/// �����ۼ��� : 2022/03/28
/// ���������� : 2022/04/10
/// ���� : 
/// 
/// �������� ����â
/// </summary>

public class StageSelectionView : MonoBehaviour
{
    public static StageSelectionView instance;
    private int _stageSelected;
    public int stageSelected
    {
        set
        {
            _stageSelected = value;
            RefreshPreviewPanel();
        }

        get
        {
            return _stageSelected;
        }
    }
    [SerializeField] private StageSelectButton stageSelectButtonPrefab;
    [SerializeField] private RectTransform contents;
    [SerializeField] private Transform doubleCheckPanel;
    [SerializeField] private Transform stagePreviewPanel;
    [SerializeField] private Transform dropItemSlotPrefab; 
    private List<StageSelectButton> stageViews = new List<StageSelectButton>();

    //===============================================================================================
    //********************************** Private Methods ********************************************
    //===============================================================================================
  
    public void RefreshPreviewPanel()
    {

        StageInfo info = StageInfoAssets.GetStageInfo(stageSelected);
        if (stagePreviewPanel.Find("StageTitle") == null)
            Debug.Log("Can't find stagetitle");
        if (stagePreviewPanel.Find("StageTitle").GetComponent<Text>()== null)
            Debug.Log("no text");
        if (info == null)
            Debug.Log($"Can't get stageinfo of {stageSelected}");
        stagePreviewPanel.Find("StageTitle").GetComponent<Text>().text = $"Chapter {info.chapter} \n Stage {info.stage}";
        stagePreviewPanel.Find("StagePreview").GetComponent<Image>().sprite = info.icon;
        Transform content = stagePreviewPanel.Find("ItemDropList").GetChild(0).GetChild(0);

        foreach (Transform child in content)
            Destroy(child.gameObject);

        foreach (var item in info.dropItemList)
        {
            Instantiate(dropItemSlotPrefab, content).GetComponent<Image>().sprite = item.icon;
            Debug.Log($"�������� {info.stage} �� ��Ӿ����� {item.name} ");
        }
            
    }

    

    //===============================================================================================
    //********************************** Private Methods ********************************************
    //===============================================================================================
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Vector2 buttonSize = stageSelectButtonPrefab.GetComponent<RectTransform>().rect.size;
        contents.GetComponent<RectTransform>().rect.Set(0, 1000, 300, StageTable.TOTAL * 100);
        int dir = 1;
        Vector2 pos = new Vector2(contents.rect.width / 4, buttonSize.y);
        int xIndex = 0;
        for (int i = 1; i <= MapInfoAssets.instance.mapInfos.Count; i++)
        {
            pos.x += (xIndex * dir * buttonSize.x);
            pos.y += buttonSize.y;

            if (i % 3 == 0)
            {
                dir *= -1;
                xIndex = 0;
            }

            xIndex++;

            StageSelectButton button = Instantiate(stageSelectButtonPrefab, contents).GetComponent<StageSelectButton>();
            button.stage = i;
            button.playDoubleCheckPanel = doubleCheckPanel;
            button.transform.position = pos;
            button.transform.GetComponentInChildren<Text>().text = i.ToString();
            stageViews.Add(button);

        }
        ActiveStageViewsOpened();

        int stageIdx = PlayerDataManager.data.GetStageLastPlayed(GameManager.characterSelected);
        if (stageIdx == 0)
            stageIdx = 1;
        stageSelected = stageIdx;
        
    }

    private void ActiveStageViewsOpened()
    {
        for (int i = 0; i < stageViews.Count; i++)
        {
            if (i > PlayerDataManager.data.GetStageSaved(GameManager.characterSelected))
                stageViews[i].isActivated = false;
            else
                stageViews[i].isActivated = true;
        }
    }    
}