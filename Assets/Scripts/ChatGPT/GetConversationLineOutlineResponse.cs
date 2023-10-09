using System.Runtime.Serialization;

namespace ChatGPT
{
    public class GetConversationLineOutlineResponse
    {
        public string SpeakerCharacterID { get; set; }
        public string SpeakerCharacterName { get; set; }
        public string NextLineOfDialogue { get; set; }

        public GetConversationLineOutlineResponse() {}

        public GetConversationLineOutlineResponse(string speakerCharacterID, string nextLineOfDialogue)
        {
            SpeakerCharacterID = speakerCharacterID;
            NextLineOfDialogue = nextLineOfDialogue;
        }
        
        public bool HasValidNextLineOfDialogue()
        { 
            return !string.IsNullOrWhiteSpace(NextLineOfDialogue);   
        }
        
        public bool HasValidSpeakerCharacterID()
        { 
            return !string.IsNullOrWhiteSpace(SpeakerCharacterID);   
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
