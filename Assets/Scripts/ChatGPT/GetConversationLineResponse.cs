using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ChatGPT
{
    [Serializable]
    public class GetConversationLineResponse
    {
        [JsonProperty("speakerId")] public string SpeakerId;
        [JsonProperty("dialogue")] public string NextLineOfDialogue;
        
        public bool HasValidNextLineOfDialogue()
        { 
            return !string.IsNullOrWhiteSpace(NextLineOfDialogue);   
        }
        
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (!string.IsNullOrWhiteSpace(NextLineOfDialogue))
            {
                NextLineOfDialogue = NextLineOfDialogue.Trim('\"');
            }
        }
    }
}