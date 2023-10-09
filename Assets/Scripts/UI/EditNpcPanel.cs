using System;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EditNpcPanel : MonoBehaviour
    {
        public event Action<NpcData> OnSubmitButtonClickedEvent; 

        [SerializeField] private TMP_InputField npcNameInputField;
        [SerializeField] private TMP_InputField npcDescriptionInputField;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button[] closeButtons;

        private NpcData _npcData;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void Initialize(NpcData npcData)
        {
            if (npcData == null)
            {
                npcData = new(string.Empty, string.Empty, string.Empty);
            }
            
            _npcData = npcData;
            npcNameInputField.text = _npcData.FullName;
            npcDescriptionInputField.text = _npcData.Description;
        }

        private void OnSubmitButtonClicked()
        {
            _npcData.SetFullName(npcNameInputField.text);
            _npcData.SetDescription(npcDescriptionInputField.text);
            OnSubmitButtonClickedEvent?.Invoke(_npcData);
            gameObject.SetActive(false);
        }

        private void OnCloseButtonClicked()
        {
            gameObject.SetActive(false);
        }
        
        private void OnEnable()
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
            for (int i = 0; i < closeButtons.Length; i++)
            {
                closeButtons[i].onClick.AddListener(OnCloseButtonClicked);
            }
        }

        private void OnDisable()
        {
            submitButton.onClick.RemoveListener(OnSubmitButtonClicked);
            for (int i = 0; i < closeButtons.Length; i++)
            {
                closeButtons[i].onClick.RemoveListener(OnCloseButtonClicked);
            }
        }
    }
}
