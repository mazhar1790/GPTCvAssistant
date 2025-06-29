using System.Collections.Generic;

namespace GPTCvAssistant.Models
{
    public class ChatModel
    {
        public string UserQuestion { get; set; }
        public List<ChatExchange> History { get; set; } = new();
        public List<string> SuggestedPrompts { get; set; } = new();

    }

    public class ChatExchange
    {
        public string UserQuestion { get; set; }
        public string Answer { get; set; }
    }
}