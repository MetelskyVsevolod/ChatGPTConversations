using System;
using System.Threading.Tasks;
using ChatGPT.Prompts;
using Helpers;
using Models;
using UnityEngine;

namespace ChatGPT
{
    public static class SetGroundLawsAndGetConversationLineHandler
    {
        public static async Task<GetConversationLineResponse> SetGroundLawsAndGetConversationLine(string chatGptApiKey, ChatGPTCommunicationHandler chatGptCommunicationHandler, ConversationRequestInputData inputData, NpcData speaker)
        {
            try
            {
                var groundLawsPrompt = string.Format(ConversationsGroundLawsPromptHolder.GroundLawsPrompt, speaker.FullName, speaker.Description);
                var messagesAmount = inputData.ConversationHistory.Count;
                var conversationHistoryString = inputData.GetConversationHistoryWithLatestDialogueLine();
                var participantsNamesAndIdsString = inputData.GetParticipantsNamesAndIdsString(speaker.Id);
                var conversationLinePrompt = string.Format(GetConversationLinePromptHolderNoPlans.GetConversationLinePrompt, conversationHistoryString, messagesAmount, participantsNamesAndIdsString, speaker.FullName);
                var messages = chatGptCommunicationHandler.GetMessagesWithConfirmationInside(groundLawsPrompt, conversationLinePrompt);

                var functionName = GroundLawsAndGetNextLineParserHolder.GetFunctionName();
                var functions = GroundLawsAndGetNextLineParserHolder.GetFunctionsNoPlans();

                Debug.Log($"{nameof(SetGroundLawsAndGetConversationLine)} ground_laws_prompt: \n{groundLawsPrompt}");
                Debug.Log($"{nameof(SetGroundLawsAndGetConversationLine)} conversation_line_prompt: \n{conversationLinePrompt}");

                for (int i = 0; i < Constants.ChatGptRequestAttempts; i++)
                {
                    try
                    {
                        chatGptCommunicationHandler.ResetChat();
                        
                        var plainResponse = await chatGptCommunicationHandler.Send(chatGptApiKey, messages);
                        
                        if (string.IsNullOrWhiteSpace(plainResponse))
                        {
                            Debug.LogError($"A {nameof(plainResponse)} for {nameof(SetGroundLawsAndGetConversationLine)} was invalid {plainResponse}");
                            continue;
                        }

                        chatGptCommunicationHandler.ResetChat();

                        var parserPrompt = string.Format(GroundLawsAndGetNextLineParserHolder.GroundLawsAndGetNextLineParserPrompt, plainResponse);
                        var jsonResponse = await chatGptCommunicationHandler.Send(chatGptApiKey, parserPrompt, functions, new FunctionCall { name = functionName });

                        if (string.IsNullOrWhiteSpace(jsonResponse))
                        {
                            Debug.LogError($"ChatGPT return an invalid JSON for {nameof(SetGroundLawsAndGetConversationLine)}: '{jsonResponse}'");
                            continue;
                        }

                        var managedToDeserialize = JsonHelper.TryDeserializeObject<GetConversationLineResponse>(jsonResponse, out var response);

                        if (!managedToDeserialize)
                        {
                            continue;
                        }

                        if (!response.HasValidNextLineOfDialogue())
                        {
                            Debug.LogError($"[{nameof(SetGroundLawsAndGetConversationLine)}] the {nameof(GetConversationLineResponse.NextLineOfDialogue)} was invalid: '{response.NextLineOfDialogue}'");
                            continue;
                        }
                        
                        Debug.Log($"{nameof(SetGroundLawsAndGetConversationLine)} parsed json response: \n{jsonResponse}");
                        response.SpeakerId = speaker.Id;
                        return response;
                    }
                    catch (Exception exception)
                    {
                        DebugHelper.LogException(nameof(SetGroundLawsAndGetConversationLine), exception);
                    }
                }
            }
            catch (Exception exception)
            {
                DebugHelper.LogException($"{nameof(SetGroundLawsAndGetConversationLineHandler)}.{nameof(SetGroundLawsAndGetConversationLine)}", exception);
            }

            return null;
        }
    }
}
