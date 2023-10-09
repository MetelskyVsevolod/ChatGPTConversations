using System;
using System.Threading.Tasks;
using ChatGPT.Prompts;
using Helpers;
using UnityEngine;

namespace ChatGPT
{
    public static class GetConversationLineOutlineHandler
    {
        public static async Task<GetConversationLineOutlineResponse> GetConversationLineOutline(string chatGptApiKey, ChatGPTCommunicationHandler chatGptCommunicationHandler, ConversationRequestInputData inputData)
        {
            try
            {
                var lastMessagesString = inputData.GetConversationHistoryWithLatestDialogueLine();
                var participantsIds = inputData.GetParticipantsIds();
                var participantsNames = inputData.GetParticipantsNames();

                var prompt = string.Format(GetConversationLineOutlinePromptHolder.GetConversationLineOutlineTemplate, lastMessagesString);
                var functions = GetConversationLineOutlinePromptHolder.GetFunctions(participantsIds, participantsNames);
                var functionName = GetConversationLineOutlinePromptHolder.GetFunctionName();
            
                Debug.Log($"{nameof(GetConversationLineOutline)} prompt: \n{prompt}");

                for (int i = 0; i < Constants.ChatGptRequestAttempts; i++)
                {
                    try
                    {                        
                        chatGptCommunicationHandler.ResetChat();
                    
                        var plainResponse = await chatGptCommunicationHandler.Send(chatGptApiKey, prompt);
                    
                        Debug.Log($"{nameof(GetConversationLineOutline)} plain response: \n{plainResponse}");
                    
                        if (string.IsNullOrWhiteSpace(plainResponse))
                        {
                            Debug.LogError($"A {nameof(plainResponse)} for {nameof(GetConversationLineOutline)} was invalid: {plainResponse}");
                            continue;
                        }
                    
                        chatGptCommunicationHandler.ResetChat();
                    
                        var parserPrompt = string.Format(GroundLawsAndGetNextLineParserHolder.GroundLawsAndGetNextLineParserPrompt, plainResponse);
                        var jsonResponse = await chatGptCommunicationHandler.Send(chatGptApiKey, parserPrompt, functions, new FunctionCall { name = functionName });
                    
                        if (string.IsNullOrWhiteSpace(jsonResponse))
                        {
                            Debug.LogError($"ChatGPT return an invalid JSON for {nameof(GetConversationLineOutline)}: '{jsonResponse}'");
                            continue;
                        }

                        var managedToDeserialize = JsonHelper.TryDeserializeObject<GetConversationLineOutlineResponse>(jsonResponse, out var response);

                        if (!managedToDeserialize)
                        {
                            continue;
                        }
                    
                        if (!ValidateSpeakerCharacterId($"{nameof(GetConversationLineOutline)}", inputData, response))
                        {
                            continue;
                        }
                    
                        Debug.Log($"{nameof(GetConversationLineOutline)} json response: \n{jsonResponse}");

                        return response;
                    }
                    catch (Exception exception)
                    {
                        DebugHelper.LogException(nameof(GetConversationLineOutline), exception); 
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.LogError($"An exception occured while trying to execute {nameof(GetConversationLineOutline)}. Message: {exception}");
            }

            return null;
        }
    
        private static bool ValidateSpeakerCharacterId(string methodName, ConversationRequestInputData inputData, GetConversationLineOutlineResponse response)
        {
            var hasValidSpeakerCharacterId = inputData.HasParticipantWithId(response.SpeakerCharacterID);

            if (!hasValidSpeakerCharacterId)
            {
                var participant = inputData.GetUniqueParticipantByFullName(response.SpeakerCharacterName);

                if (participant != null)
                {
                    Debug.Log($"[{methodName}] the {nameof(GetConversationLineOutlineResponse.SpeakerCharacterID)} was set using full name: '{response.SpeakerCharacterName}'");
                    response.SpeakerCharacterID = participant.Id;
                }
                else
                {
                    Debug.Log($"[{methodName}] the {nameof(GetConversationLineOutlineResponse.SpeakerCharacterID)} was invalid: '{response.SpeakerCharacterID}'");
                    return false;
                }
            }

            return true;
        }
    }
}
