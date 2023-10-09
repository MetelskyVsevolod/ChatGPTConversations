using System;

namespace ChatGPT 
{
    [Serializable]
    public class ChatGPTResultError
    {
        public Error error;
    }

    [Serializable]
    public class Error 
    {
        public string message;
    }
}