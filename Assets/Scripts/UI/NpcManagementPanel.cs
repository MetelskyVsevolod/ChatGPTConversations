using System;
using System.Collections.Generic;
using System.Linq;
using Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NpcManagementPanel : MonoBehaviour
    {
        public event Action<List<NpcData>> OnNpcListUpdatedEvent; 

        [SerializeField] private Button createNewNpcButton;
        [SerializeField] private RectTransform npcListParent;
        [SerializeField] private NpcView npcViewPrefab;
        [SerializeField] private EditNpcPanel editNpcPanel;
        [SerializeField] private Button[] closeButtons;

        private List<NpcData> _npcData;
        private List<NpcView> _npcViews;

        private void Awake()
        {
            gameObject.SetActive(false);
        }
        
        public void Initialize(List<NpcData> npcData)
        {
            _npcData = npcData;
            RefreshViews();
        }

        private void RefreshViews()
        {
            if (_npcViews != null)
            {
                for (int i = 0; i < _npcViews.Count; i++)
                {
                    _npcViews[i].OnDeleteButtonClickedEvent -= OnDeleteButtonClicked;
                    _npcViews[i].OnEditButtonClickedEvent -= OnEditButtonClicked;
                    Destroy(_npcViews[i].gameObject);
                }
            }

            if (_npcViews == null)
            {
                _npcViews = new();
            }
            
            _npcViews.Clear();
            
            for (int i = 0; i < _npcData.Count; i++)
            {
                SpawnNpcView(_npcData[i]);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(npcListParent);
        }

        private void SpawnNpcView(NpcData npcData)
        {
            var spawnedView = Instantiate(npcViewPrefab, npcListParent);
            spawnedView.Initialize(npcData);
            _npcViews.Add(spawnedView);
                
            spawnedView.OnDeleteButtonClickedEvent += OnDeleteButtonClicked;
            spawnedView.OnEditButtonClickedEvent += OnEditButtonClicked;
        }
        
        private void OnDeleteButtonClicked(NpcData npcData)
        {
            _npcData.Remove(npcData);
            
            for (int i = 0; i < _npcViews.Count; i++)
            {
                var isTargetView = npcData.Id == _npcViews[i].NpcData.Id;

                if (isTargetView)
                {
                    Destroy(_npcViews[i].gameObject);
                    _npcViews.RemoveAt(i);
                    break;
                }
            }
            
            OnNpcListUpdatedEvent?.Invoke(_npcData);
        }
        
        private void OnEditButtonClicked(NpcData npcData)
        {
            editNpcPanel.gameObject.SetActive(true);
            editNpcPanel.Initialize(npcData);
        }

        private void OnSubmitButtonClicked(NpcData npcData)
        {
            var existingNpc = _npcData.FirstOrDefault(npc => string.Equals(npc.Id, npcData.Id));

            if (existingNpc != null)
            {
                existingNpc.SetFullName(npcData.FullName);
                existingNpc.SetDescription(npcData.Description);
            }
            else
            {
                var id = (_npcData.Count + 1).ToString();
                npcData.SetId(id);
                _npcData.Add(npcData);
                SpawnNpcView(npcData);
                OnNpcListUpdatedEvent?.Invoke(_npcData);
            }
        }

        private void OnCloseButtonClicked()
        {
            gameObject.SetActive(false);
        }

        private void OnCreateNewNpcButtonClicked()
        {
            editNpcPanel.gameObject.SetActive(true);
            editNpcPanel.Initialize(null);
        }
        
        private void OnEnable()
        {
            for (int i = 0; i < closeButtons.Length; i++)
            {
                closeButtons[i].onClick.AddListener(OnCloseButtonClicked);
            }
            
            createNewNpcButton.onClick.AddListener(OnCreateNewNpcButtonClicked);
            editNpcPanel.OnSubmitButtonClickedEvent += OnSubmitButtonClicked;
        }

        private void OnDisable()
        {
            for (int i = 0; i < closeButtons.Length; i++)
            {
                closeButtons[i].onClick.RemoveListener(OnCloseButtonClicked);
            }
            
            createNewNpcButton.onClick.RemoveListener(OnCreateNewNpcButtonClicked);
            editNpcPanel.OnSubmitButtonClickedEvent -= OnSubmitButtonClicked;
        }
    }
}
