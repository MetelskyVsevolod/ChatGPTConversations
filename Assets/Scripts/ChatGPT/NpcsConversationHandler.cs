using System;
using System.Threading.Tasks;
using Helpers;

namespace ChatGPT
{
    public static class NpcsConversationHandler
    {
        private static readonly ChatGPTCommunicationHandler _chatGptCommunicationHandler;
        
        static NpcsConversationHandler()
        {
            _chatGptCommunicationHandler = new ChatGPTCommunicationHandler();
        }
        
        public static async Task<GetConversationLineResponse> GetConversationLine(string chatGptApiKey, ConversationRequestInputData inputData)
        {
            for (int i = 0; i < Constants.ChatGptRequestAttempts; i++)
            {
                try
                {
                    var conversationLineOutline = await GetConversationLineOutlineHandler.GetConversationLineOutline(chatGptApiKey, _chatGptCommunicationHandler, inputData);

                    var speakerCharacterID = conversationLineOutline.SpeakerCharacterID;

                    if (string.IsNullOrWhiteSpace(speakerCharacterID))
                    {
                        continue;
                    }
                    
                    var speakerNpcData = inputData.GetParticipantById(speakerCharacterID);

                    if (speakerNpcData == null)
                    {
                        continue;
                    }
                    
                    var conversationLine = await SetGroundLawsAndGetConversationLineHandler.SetGroundLawsAndGetConversationLine(chatGptApiKey, _chatGptCommunicationHandler, inputData, speakerNpcData);

                    if (conversationLine == null)
                    {
                        continue;
                    }

                    return conversationLine;
                }
                catch (Exception exception)
                {
                    DebugHelper.LogException($"{nameof(NpcsConversationHandler)}.{nameof(GetConversationLine)}", exception);
                }
            }

            return null;
        }
    }
}
