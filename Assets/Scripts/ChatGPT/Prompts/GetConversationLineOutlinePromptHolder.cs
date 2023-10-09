namespace ChatGPT.Prompts
{
    public static class GetConversationLineOutlinePromptHolder
    {
        public static string GetConversationLineOutlineTemplate =
@"### Instruction ###
You are the main character in ""Egg Theory"", who currently controls the conversation between a few people (all of them are you) and can see the memories that they recall:
You shall never refer to the egg theory in your outputs!!!

NOTE: Each person is identified in the format ""[Name]([ID])"".
Here are the last few messages from a conversation ordered with the most recent message at the bottom:
'''
{0}
'''

Reason about the most recent dialogue and context from other dialogues.

Based on the reasoning, as the main character, mention the name of the character who should speak next in the conversation, also predict what the person would say, this must be a person whose relevant memories are given above.
More rules:
- The same person must refrain from repeating the most recent dialogue or repeating its context.
- The conversation must remain coherent, i.e. there must be no abrupt changes of topic!

Dialogues must also be influenced by the last dialogue in the conversation, and should continue the conversation smoothly either based on recalled memories or improvising upon the context of latest dialogue.

Output format:
'''
<ConversationSummary> (example: ""the conversation so far involved ...., then ... said, ... and ..., the most recent dialogue ..."")
<LastDialogueAnalysis> (example: ""the most recent dialogue was: ... by .."")
<LastSpeakerName>
<RelevantMemories> (example: "".. remembers: ..., and ... recalls ..., ..."")
<Reasoning> (example: ""the conversation so far has.... by .., .. spoke recently, it would makes sense for ... based on ... (OR) based on their relevant memory, therefore, ... can speak next"")
<SpeakerCharacterID>
<SpeakerCharacterName>
'''

Use the logic given in the function description to generate output.
DO NOT USE THE EXACT WORDS FROM EXAMPLES FOR THE OUTPUT.
 
";
        public static string GetFunctionName()
        {
            return "GetNextLineOfDialogue";
        }
        
        public static dynamic[] GetFunctions(string[] npcsIds, string[] npcsNames)
        {
            var functionDescription = new
            {
                name = GetFunctionName(),
                description = "Gets the next line of dialogue, the speaker character name, the reasoning and relevant memories",
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        LastDialogueAnalysis =
                        new
                        {
                            type = "string",
                            description = "The last line of dialogue that was said. For example 'Sup guys! How are you doing?'"
                        },
                        LastSpeakerName =
                        new
                        {
                            type = "string",
                            description = "The name of the recent speaker, who spoke the last dialogue"
                        },
                        Reasoning =
                        new
                        {
                            type = "string",
                            description = "Reasoning to determine next dialogue in the conversation with analysis of the context of the conversation history, with focus on the context of the most recent message. Example: '_ said _, based on this, _ can say _'"
                        },
                        SpeakerCharacterID =
                        new
                        {
                            type = "string",
                            description = "The id of a character who is going to say the next line of dialogue",
                            @enum = npcsIds
                        },
                        SpeakerCharacterName =
                        new
                        {
                            type = "string",
                            description = "The name of a character who is going to say the next line of dialogue",
                            @enum = npcsNames
                        },
                        NextLineOfDialogue = new
                        {
                            type = "string",
                            description = "A dialogue that the person might speak"
                        }
                    },
                    required = new[]
                    {
                        "LastDialogueAnalysis", "LastSpeakerName", "Reasoning",
                        "SpeakerCharacterID", "SpeakerCharacterName", "NextLineOfDialogue"
                    }
                }
            };

            return new dynamic[] {functionDescription};
        }
    }
}
