using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Quests : MonoBehaviour
{
    [SerializeField] private QuestRecord _prefab;
    [SerializeField] private Transform _contentHolder;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;

    private ICitiesGetter _citiesGetter;
    private Dictionary<Vector2Int, string> _citiesNames;
    private bool _isInited;
    private List<QuestRecord> _quests = new List<QuestRecord>();

    private void Awake()
    {
        _openButton.onClick.AddListener(FillQuestboard);    
        _closeButton.onClick.AddListener(CleanQuestboard);
    }

    private void OnDestroy()
    {
        _openButton.onClick.RemoveListener(FillQuestboard);
        _closeButton.onClick.RemoveListener(CleanQuestboard);
    }

    public void Init(ICitiesGetter citiesGetter, SerializedPair<Vector2Int, string>[] citiesNames)
    {
        _citiesGetter = citiesGetter != null ? citiesGetter : throw new ArgumentNullException(nameof(citiesGetter));

        if (citiesNames == null)
            throw new ArgumentNullException(nameof(citiesNames));

        _citiesNames = citiesNames.ToDictionary(key => key.Key, value => value.Value);
        _isInited = true;
    }

    private void FillQuestboard()
    {
        if (_isInited == false)
            return;

        var cities = _citiesGetter.GetCities();

        foreach (var city in _citiesNames.Keys)
        {
            var record = Instantiate(_prefab, _contentHolder);
            _quests.Add(record);
            bool isCompleted = cities[city] == Side.Player ? true : false;
            record.Init(_citiesNames[city], isCompleted);
        }
    }

    private void CleanQuestboard()
    {
        if (_isInited == false)
            return;

        for (int i = 0; i < _quests.Count; i++)
        {
            _quests[i].gameObject.SetActive(false);
            Destroy(_quests[i].gameObject);
        }

        _quests.Clear();
    }
}
