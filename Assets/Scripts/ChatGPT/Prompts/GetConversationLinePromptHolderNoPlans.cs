namespace ChatGPT.Prompts
{
    public static class GetConversationLinePromptHolderNoPlans
    {
        public static string GetConversationLinePrompt = @"
Current participants of the conversation (identified in the format ""[Name]([ID])"")
'''
{0}
'''

<Conversation history>
'''
{1}
'''

Dialogues in the convo so far: {2}

Reason about the emotions experienced by {3} and the observations of surroundings, which could be reasoning about what {2}'s action and related dialogue spoken be in the situation be (such that it fits well as a new addition to <Conversation history>).

Then output in the format:
```
<World reasoning>
<Past dialogues reasoning>
<Latest dialogue reasoning>
<Role reasoning>
<Additional reasoning>
<Action reasoning> 
<Action name> 
<Dialogue>
```
Keep `<Dialogue>` to at most 100 words
";

    }
}