using System;
using System.Collections.Generic;
using System.Linq;
using Models;
using Newtonsoft.Json;

namespace ChatGPT
{
    [Serializable]
    public class ConversationRequestInputData
    {
        [JsonProperty] public List<string> ConversationHistory { get; private set; }
        [JsonProperty] public IEnumerable<NpcData> Participants { get; private set; }

        public ConversationRequestInputData() {}

        public ConversationRequestInputData(List<string> conversationHistory, IEnumerable<NpcData> participants)
        {
            ConversationHistory = conversationHistory;
            Participants = participants;
            
            if (conversationHistory == null)
            {
                ConversationHistory = new List<string>();
            }

            if (participants == null)
            {
                Participants = Array.Empty<NpcData>();
            }
        }

        public void AddConversationMessage(string message)
        {
            ConversationHistory.Add(message);
        }
        
        public string GetConversationHistoryString()
        {
            return GetConversationHistoryString(ConversationHistory);
        }

        public static string GetConversationHistoryString(IEnumerable<string> conversationHistory)
        {
            var validHistoryEntries = conversationHistory.Where(entry => !string.IsNullOrWhiteSpace(entry));
            return string.Join("; ", validHistoryEntries);
        }
        
        public string GetConversationHistoryWithLatestDialogueLine(string lastDialogue = null)
        {
            var lastMessagesString = string.Empty;
            for (int i = 0; i < ConversationHistory.Count - 1; i++)
            {
                if (ConversationHistory[i] == null)
                {
                    continue;
                }

                lastMessagesString += $"{ConversationHistory[i]}";
                lastMessagesString += "\n";
            }

            if (string.IsNullOrWhiteSpace(lastDialogue))
            {
                lastDialogue = ConversationHistory.Last();
            }
            
            lastMessagesString += "\n[Latest dialogue]:\n";
            lastMessagesString += $"{lastDialogue}";
            return lastMessagesString;
        }
        
        public string GetParticipantsNamesAndIdsString(string speakerId)
        {
            var validParticipants = Participants.Where(participant => participant != null).ToArray();
            var result = string.Empty;

            for (int i = 0; i < validParticipants.Length; i++)
            {
                var participant = validParticipants[i];
                result += $"{participant.FullName} ({participant.Id})";

                var isSpeaker = string.Equals(participant.Id, speakerId, StringComparison.OrdinalIgnoreCase);

                if (isSpeaker)
                {
                    result += $" (this is {participant.FullName} itself)";
                }

                var isLast = i == validParticipants.Length - 1;

                if (!isLast)
                {
                    result += "\n";
                }
            }
            return result;
        }
        
        public string[] GetParticipantsNames()
        {
            var validParticipants = Participants.Where(participant => participant != null);
            return validParticipants.Select(participant => participant.FullName).ToArray();
        }
        
        public string[] GetParticipantsNamesForEnum()
        {
            var validParticipants = Participants.Where(participant => participant != null);
            return validParticipants.Select(participant => participant.FullName.Replace(' ', '_')).ToArray();
        }
        
        public string[] GetParticipantsIds()
        {
            var validParticipants = Participants.Where(participant => participant != null);
            return validParticipants.Select(participant => participant.Id).ToArray();
        }

        public NpcData GetUniqueParticipantByFullName(string participantFullName)
        {
            var participants = Participants.Where(character => string.Equals(character.FullName, participantFullName, StringComparison.OrdinalIgnoreCase)).ToArray();

            if (participants.Length != 1)
            {
                return null;
            }

            return participants.First();
        }
        
        public NpcData GetParticipantById(string participantId)
        {
            return Participants.FirstOrDefault(character => string.Equals(character.Id, participantId));
        }

        public bool HasParticipantWithId(string participantId)
        {
            return GetParticipantById(participantId) != null;
        }
    }
}
