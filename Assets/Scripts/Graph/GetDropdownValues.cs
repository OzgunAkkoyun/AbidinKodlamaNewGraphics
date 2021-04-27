using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetDropdownValues : MonoBehaviour
{

    [SerializeField] TMP_Dropdown senarioDropDown;
    [SerializeField] TMP_Dropdown levelDropDown;
    [SerializeField] TMP_Dropdown subLevelDropDown;

    public WindowGraph windowGraph;
    public LevelLoader levelLoader;

    public TextMeshProUGUI senarioStatsText;
    public GameObject statPanel;
    void OnEnable()
    {
        Invoke("ShowFullLevelPercent", .2f);
    }

    private void ShowFullLevelPercent()
    {
        var allLevels = levelLoader.currentLevelStats;
        var complatedAllLevelPercent = allLevels.GetComplatedAllLevelPercent();
        var complatedLevelPercentBySenario = allLevels.GetComplatedLevelPercentBySenario();

        List<float> data = new List<float>(){complatedAllLevelPercent};
        windowGraph.StartShowGraph(data);
        windowGraph.ShowPieGraph(complatedLevelPercentBySenario);
    }
    private void ShowValues()
    {
        var data = SaveLoadUserData.instance.GetSpecificUserData(senarioDropDown.value + 1,
            levelDropDown.value + 1, subLevelDropDown.value + 1);
        windowGraph.StartShowGraph(data);
    }

    public void StatPanelClose()
    {
        statPanel.SetActive(false);
    }
    public void ShowSenarioStats(int senarioIndex)
    {
        statPanel.SetActive(true);
        GetLevelPlayedCount(senarioIndex);
    }
    
    private void GetLevelPlayedCount(int senarioIndex)
    {
        var levelOnePlayedCount = SaveLoadUserData.instance.HowManyTimeLevelPlayed(senarioIndex, 1);
        var levelTwoPlayedCount = SaveLoadUserData.instance.HowManyTimeLevelPlayed(senarioIndex, 2);
        var levelThreePlayedCount = SaveLoadUserData.instance.HowManyTimeLevelPlayed(senarioIndex, 3);

        var levelOneTF = SaveLoadUserData.instance.LevelTrueFalse(senarioIndex, 1);
        var levelTwoTF = SaveLoadUserData.instance.LevelTrueFalse(senarioIndex, 2);
        var levelThreeTF = SaveLoadUserData.instance.LevelTrueFalse(senarioIndex, 3);

        var totalTime = SaveLoadUserData.instance.SenarioTotalTime(senarioIndex);
        string clockTotal = TimeSpan.FromSeconds(totalTime).ToString("hh':'mm':'ss");

        var levelOneTimeTotal = SaveLoadUserData.instance.LevelTotalTime(senarioIndex, 1);
        string clockOne = TimeSpan.FromSeconds(levelOneTimeTotal).ToString("hh':'mm':'ss");

        var levelTwoTimeTotal = SaveLoadUserData.instance.LevelTotalTime(senarioIndex, 2);
        string clockTwo = TimeSpan.FromSeconds(levelTwoTimeTotal).ToString("hh':'mm':'ss");

        var levelThreeTimeTotal = SaveLoadUserData.instance.LevelTotalTime(senarioIndex, 3);
        string clockThree = TimeSpan.FromSeconds(levelThreeTimeTotal).ToString("hh':'mm':'ss");

        string levelOnePer = levelOnePlayedCount != 0 ? ((float)levelOneTF[0] / levelOnePlayedCount * 100).ToString("F2") : 0f.ToString();
        string levelTwoPer = levelTwoPlayedCount != 0 ? ((float)levelTwoTF[0] / levelTwoPlayedCount * 100).ToString("F2") : 0f.ToString();
        string levelThreePer = levelThreePlayedCount != 0 ? ((float)levelThreeTF[0] / levelThreePlayedCount * 100).ToString("F2") : 0f.ToString();

        senarioStatsText.text = "Senaryo " + senarioIndex + " verileri.\n";
        senarioStatsText.text += "Senaryo " + senarioIndex + "'de geçirilen toplam süre: "+ clockTotal + " saat.\n\n";

        senarioStatsText.text += "1. Level: \n";
        senarioStatsText.text += "Oynanma süresi: " + clockOne + " saat\n";
        senarioStatsText.text += "Oynanma istatistiği: " + levelOnePlayedCount + "kez oynanmış\n";
        senarioStatsText.text += "Doğru / yanlış oranı: " + levelOneTF[0] + " / " + levelOneTF[1] + ". Başarı oranı: %" + levelOnePer + "\n\n";

        senarioStatsText.text += "2. Level: \n";
        senarioStatsText.text += "Oynanma süresi: " + clockTwo + " saat\n";
        senarioStatsText.text += "Oynanma istatistiği: " + levelTwoPlayedCount + "kez oynanmış\n";
        senarioStatsText.text += "Doğru / yanlış oranı: " + levelTwoTF[0] + " / " + levelTwoTF[1] + ". Başarı oranı: %" + levelTwoPer + "\n\n";

        senarioStatsText.text += "3. Level: \n";
        senarioStatsText.text += "Oynanma süresi: " + clockThree + " saat\n";
        senarioStatsText.text += "Oynanma istatistiği: " + levelThreePlayedCount + "kez oynanmış\n";
        senarioStatsText.text += "Doğru / yanlış oranı: " + levelThreeTF[0] + " / " + levelThreeTF[1] + ". Başarı oranı: %" + levelThreePer + "\n\n";

    }

    string SetDisplayText(int senario, int level, int subLevel)
    {
        return (senario + 1) + ". Senaryo " + (level + 1) + ". bölümün " + (subLevel + 1) +
               ". alt bölümü verileri.";
    }


}
