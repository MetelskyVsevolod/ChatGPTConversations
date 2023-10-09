using System.Collections.Generic;

namespace ChatGPT
{
    public class ChatGPTChat
    {
        public List<Message> CurrentChat { get; } = new();

        public void AddMessage(string speaker, string messageText)
        {
            var message = new Message(speaker, messageText);
            CurrentChat.Add(message);
        }
        
        public void AddUserMessage(string messageText)
        {
            AddMessage(Constants.ChatGPTUser, messageText);
        }
        
        public void AddAssistantMessage(string messageText)
        {
            AddMessage(Constants.ChatGPTAssistant, messageText);
        }
    }
}
