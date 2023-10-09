using System.Collections.Generic;

namespace ChatGPT
{
    public class ChatGPTRequest
    {
        public string model;
        public List<Message> messages;
        public object[] functions;
        public FunctionCall function_call;
        public double temperature;
    }
}