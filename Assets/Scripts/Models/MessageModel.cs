using System;
using Newtonsoft.Json;

namespace Models
{
    [Serializable]
    public class MessageModel
    {
        [JsonProperty] public string SpeakerNameText { get; private set; }
        [JsonProperty] public string MessageText { get; private set; }

        public MessageModel(string speakerNameText, string messageText)
        {
            SpeakerNameText = speakerNameText;
            MessageText = messageText;
        }
    }
}
