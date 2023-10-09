using System;

namespace ChatGPT 
{
    [Serializable]
    public class Message 
    {
        public string role;
        public string content;
        public FunctionCall function_call;

        public Message(string role, string content) 
        {
            this.role = role;
            this.content = content;
        }
    }
}