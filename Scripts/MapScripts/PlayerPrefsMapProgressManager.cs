
using UnityEngine;

public class PlayerPrefsMapProgressManager : IMapProgressManager
{
    private string GetLevelKey(int number)
    {
        // Debug.Log("PlayerPrefsMapProgressManager : IMapProgressManager GetLevelKey: " + string.Format("Level.{0:000}.StarsCount", number));
        return string.Format("Level.{0:000}.StarsCount", number);
    }

    //VIP Function
    //这个会在刚进入Game Scene和打完关卡后被疯狂的调用
    public int LoadLevelStarsCount(int level)
    {
        // Debug.Log("PlayerPrefsMapProgressManager : IMapProgressManager LoadLevelStarsCount: " + PlayerPrefs.GetInt(GetLevelKey(level), 0));
        return PlayerPrefs.GetInt(GetLevelKey(level), 0);
    }

    public void SaveLevelStarsCount(int level, int starsCount)
    {
        Debug.Log("PlayerPrefsMapProgressManager : IMapProgressManager SaveLevelStarsCount: " + string.Format("Stars count {0} of level {1} saved.", starsCount, level));
        PlayerPrefs.SetInt(GetLevelKey(level), starsCount);
    }

    public void ClearLevelProgress(int level)
    {
        PlayerPrefs.DeleteKey(GetLevelKey(level));
    }
}
