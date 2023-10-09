using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatGPT;
using Helpers;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NpcConversationPanel : MonoBehaviour
    {
        [SerializeField] private Button nextButton;
        [SerializeField] private Button openNpcManagementPanelButton;
        [SerializeField] private MessageView messageViewPrefab;
        [SerializeField] private RectTransform messageViewsParent;
        [SerializeField] private NpcManagementPanel npcManagementPanel;
        [SerializeField] private TMP_InputField chatGptApiKeyInputField;

        private List<NpcData> _participants = new();
        private readonly List<string> _conversationHistory = new();
        private string _encryptionPassword = "VMetelskyi";

        private void Awake()
        {
            AddParticipant(new NpcData("1", "James Bond", "A world famous secret agent 007"));
            AddParticipant(new NpcData("2", "Spiderman", "A Marvel superhero"));
            AddParticipant(new NpcData("3", "Anakin Skywalker", "A jedi knight, the Chosen One"));
            chatGptApiKeyInputField.text = string.Empty;
        }

        private string GetApiKey()
        {
            if (string.IsNullOrWhiteSpace(chatGptApiKeyInputField.text))
            {
                return "QG0t55xpFutIMsWms9jUsFqebMAhxvQ8/NFsE6A49F9nvqTEsKE92ZOwlhcGarIcxdD7wJFEpaXHvr9peUxesQ==".DecryptString(_encryptionPassword);
            }

            return chatGptApiKeyInputField.text;
        }
        
        private void AddParticipant(NpcData npcData)
        {
            var historyMessage = $"{npcData.FullName} ({npcData.Id}) has joined a conversation";
            _conversationHistory.Add(historyMessage);
            _participants.Add(npcData);
        }

        private void OnOpenNpcManagementPanelButtonClicked()
        {
            npcManagementPanel.gameObject.SetActive(true);
            npcManagementPanel.Initialize(_participants);
        }
        
        private void OnNextButtonClicked()
        {
            RequestNextMessageFromNpcs();
        }

        private async Task RequestNextMessageFromNpcs()
        {
            try
            {
                nextButton.interactable = false;
                openNpcManagementPanelButton.interactable = false;

                var conversationRequestInputData = new ConversationRequestInputData(_conversationHistory, _participants);
                var conversationLine = await NpcsConversationHandler.GetConversationLine(GetApiKey(), conversationRequestInputData);
                var speakerNpcData = conversationRequestInputData.GetParticipantById(conversationLine.SpeakerId);
                var historyString = $"{speakerNpcData.FullName} ({speakerNpcData.Id}): {conversationLine.NextLineOfDialogue}";
                _conversationHistory.Add(historyString);

                var messageModel = new MessageModel(speakerNpcData.FullName, conversationLine.NextLineOfDialogue);
                var spawnedMessageView = Instantiate(messageViewPrefab, messageViewsParent);
                spawnedMessageView.Initialize(messageModel);
                LayoutRebuilder.ForceRebuildLayoutImmediate(messageViewsParent);
                
                nextButton.interactable = true;
                openNpcManagementPanelButton.interactable = true;
            }
            catch (Exception exception)
            {
                DebugHelper.LogException($"{nameof(NpcConversationPanel)}.{nameof(RequestNextMessageFromNpcs)}", exception);
            }
        }

        private void OnNpcListUpdated(List<NpcData> npcData)
        {
            _participants = npcData;
        }
        
        private void OnEnable()
        {
            openNpcManagementPanelButton.onClick.AddListener(OnOpenNpcManagementPanelButtonClicked);
            nextButton.onClick.AddListener(OnNextButtonClicked);
            npcManagementPanel.OnNpcListUpdatedEvent += OnNpcListUpdated;
        }
        
        private void OnDisable()
        {
            openNpcManagementPanelButton.onClick.AddListener(OnOpenNpcManagementPanelButtonClicked);
            nextButton.onClick.RemoveListener(OnNextButtonClicked);
            npcManagementPanel.OnNpcListUpdatedEvent -= OnNpcListUpdated;
        }
    }
}
