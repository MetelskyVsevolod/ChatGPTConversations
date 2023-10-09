using System;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NpcView : MonoBehaviour
    {
        public event Action<NpcData> OnEditButtonClickedEvent; 
        public event Action<NpcData> OnDeleteButtonClickedEvent; 

        [SerializeField] private Button editButton;
        [SerializeField] private Button deleteButton;
        [SerializeField] private TMP_Text npcNameText;
        [SerializeField] private TMP_Text npcDescriptionText;

        public NpcData NpcData { get; private set; }

        public void Initialize(NpcData npcData)
        {
            if (NpcData != null)
            {
                NpcData.OnFullNameChangedEvent -= OnFullNameChanged;
                NpcData.OnDescriptionChangedEvent -= OnDescriptionChanged;
            }

            NpcData = npcData;
            RefreshFullName();
            RefreshDescription();
            
            NpcData.OnFullNameChangedEvent += OnFullNameChanged;
            NpcData.OnDescriptionChangedEvent += OnDescriptionChanged;
        }

        private void RefreshFullName()
        {
            npcNameText.text = NpcData.FullName;
        }

        private void RefreshDescription()
        {
            npcDescriptionText.text = NpcData.Description;
        }
        
        private void OnFullNameChanged()
        {
            RefreshFullName();
        }

        private void OnDescriptionChanged()
        {
            RefreshDescription();
        }

        private void OnEditButtonClicked()
        {
            OnEditButtonClickedEvent?.Invoke(NpcData);
        }

        private void OnDeleteButtonClicked()
        {
            OnDeleteButtonClickedEvent?.Invoke(NpcData);
        }

        private void OnEnable()
        {
            editButton.onClick.AddListener(OnEditButtonClicked);
            deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        }
        
        private void OnDisable()
        {
            editButton.onClick.RemoveListener(OnEditButtonClicked);
            deleteButton.onClick.RemoveListener(OnDeleteButtonClicked);
        }
    }
}
