using System.Collections.Generic;
using UnityEngine;

public class GameProgress : Singleton<GameProgress>
{
    public GameObject iceCaveLevelSet;

    public Stack<Mermaid.Variation> artifacts = new Stack<Mermaid.Variation>();    // mermaid haircolor = artifact color

    internal int dungeonsFinished = 0;
    internal bool hasCollectedNewGem { get; set; }

    [Tooltip("If 1, Enable all Portal PlayerPrefs (Only in Editor). If 0, Reset all Portal PlayerPrefs")]
    public int portalPrefs = 1;

    private void Start()
    {
        PlayerPrefs.SetInt("IceCavePortal", portalPrefs);
        PlayerPrefs.SetInt("DungeonPortal", portalPrefs);

        PlayerHealth.Instance.onDeath += ResetGameProgress;

    }
    private void OnDisable()
    {
        PlayerHealth.Instance.onDeath -= ResetGameProgress;
    }

    internal void AddArtifact(Mermaid.Variation m)
    {
        artifacts.Push(m);
        hasCollectedNewGem = true;
    }
    internal void ResetGameProgress()
    {
        artifacts.Clear();
        dungeonsFinished = 0;
    }

    internal void AddIceCaveLevelSet()
    {
        Debug.Log("Add Ice Cave LevelSet");
        Ocean_MapHandler.Instance.TryAddLevelSet(iceCaveLevelSet);
    }
}
