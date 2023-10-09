using System;
using System.Collections.Generic;
using ChatGPT;

namespace ChatGPTWrapper {
    [Serializable]
    public class ChatGPTResult
    {
        public List<ChatGPTChoices> choices;
        public string ErrorMessage;
    }
}