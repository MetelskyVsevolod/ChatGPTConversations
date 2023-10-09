using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatGPTWrapper;
using Helpers;
using Formatting = Newtonsoft.Json.Formatting;

namespace ChatGPT
{
    public class ChatGPTCommunicationHandler
    {
        private readonly string _selectedModel = Constants.ChatGpt35Model;
        
        private ChatGPTChat _chatGptChat;
        private readonly RequestsHandler _requests = new();
        private string _uri = "https://api.openai.com/v1/chat/completions";
        private readonly List<(string, string)> _requestHeaders;
        
        public ChatGPTCommunicationHandler()
        {
            _chatGptChat = new ChatGPTChat();
            _requestHeaders = new List<(string, string)>
            {
                ("Content-Type", "application/json")
            };
        }

        public async Task<string> Send(string chatGptApiKey, string messageText, object[] functions = null, FunctionCall functionCall = null, string modelOverride = null)
        {
            try
            {
                _chatGptChat.AddUserMessage(messageText);
                return await SendInternal(chatGptApiKey, functions, functionCall, modelOverride);
            }
            catch (Exception exception)
            {
                DebugHelper.LogException($"{nameof(ChatGPTCommunicationHandler)}.{nameof(Send)}", exception);
            }

            return null;
        }

        public async Task<string> Send(string chatGptApiKey, Tuple<string, string>[] messages, object[] functions = null, FunctionCall functionCall = null, string modelOverride = null, double temperature = 1.2)
        {
            try
            {
                for (int i = 0; i < messages.Length; i++)
                {
                    var speaker = messages[i].Item1;
                    var text = messages[i].Item2;
                    _chatGptChat.AddMessage(speaker, text);
                }
            
                return await SendInternal(chatGptApiKey, functions, functionCall, modelOverride, temperature);
            }
            catch (Exception exception)
            {
                DebugHelper.LogException($"{nameof(ChatGPTCommunicationHandler)}.{nameof(Send)}", exception);
            }

            return null;
        }

        private async Task<string> SendInternal(string chatGptApiKey, object[] functions = null, FunctionCall functionCall = null, string modelOverride = null, double temperature = 1.2)
        {
            try
            {
                var modelToUse = _selectedModel;

                if (!string.IsNullOrWhiteSpace(modelOverride))
                {
                    modelToUse = modelOverride;
                }
            
                var request = new ChatGPTRequest {model = modelToUse, messages = _chatGptChat.CurrentChat, functions = functions, function_call = functionCall, temperature = temperature};
                var json = JsonHelper.SerializeObject(request);
                _requestHeaders.Add(("Authorization", $"Bearer {chatGptApiKey}"));
                var response = await _requests.PostRequest<ChatGPTResult>( _uri, json, _requestHeaders);
                
                if (response.Item1 == null || !string.IsNullOrWhiteSpace(response.Item2))
                {
                    LogErrors(request);
                    return null;
                }

                var responseMessage = response.Item1.choices[0].message.content;

                if (string.IsNullOrWhiteSpace(responseMessage))
                {
                    if (response.Item1.choices == null || response.Item1.choices.Count == 0)
                    {
                        return null;
                    }

                    if (response.Item1.choices[0].message == null)
                    {
                        return null;
                    }
                    
                    if (response.Item1.choices[0].message.function_call == null)
                    {
                        return null;
                    }
                    
                    return response.Item1.choices[0].message.function_call.arguments;
                }
                
                return responseMessage;
            }
            catch (Exception exception)
            {
                DebugHelper.LogException($"{nameof(ChatGPTCommunicationHandler)}.{nameof(SendInternal)}", exception);
            }
            
            return null;
        }

        public Tuple<string, string>[] GetMessagesWithConfirmationInside(string firstPrompt, string secondPrompt)
        {
            return GetMessagesWithConfirmationInside(firstPrompt, "TALK: \"Ok\"", secondPrompt);
        }
        
        public Tuple<string, string>[] GetMessagesWithConfirmationInside(string firstPrompt, string message, string secondPrompt)
        {
            var messages = new[]
            {
                Tuple.Create(Constants.ChatGPTUser, firstPrompt),
                Tuple.Create(Constants.ChatGPTAssistant, message),
                Tuple.Create(Constants.ChatGPTUser, secondPrompt)
            };
            
            return messages;
        }
        
        private void LogErrors(ChatGPTRequest request)
        {
            var logJson = JsonHelper.SerializeObject(request, Formatting.Indented);
            
            var errorMessage = "==An error has occured while trying to send a message to ChatGPT==";
            errorMessage += "\n=Chat history=";

            foreach (var msg in _chatGptChat.CurrentChat)
            {
                errorMessage += $"\n={msg.role}\n{msg.content}\n{msg.function_call}=";
            }

            errorMessage += $"\n=JSON in the last request: {logJson}=";
            UnityEngine.Debug.LogError(errorMessage);
        }
        
        public void ResetChat()
        {
            _chatGptChat = new();
        }
    }
}
