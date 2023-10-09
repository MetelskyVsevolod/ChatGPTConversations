namespace ChatGPT.Prompts
{
    public static class GroundLawsAndGetNextLineParserHolder
    {
        public static string GroundLawsAndGetNextLineParserPrompt = @"Parse the following input into the JSON with the schema given in the function below: ""{0}"" ";
        
        public static string GetFunctionName()
        {
            return "GetDetailedLineOfDialogue";
        }

        public static dynamic[] GetFunctionsNoPlans()
        {
            var functionDescription = new
            {
                name = GetFunctionName(),
                description = "Gets the action type and text if that's a dialogue line",
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        speaker_name = new
                        {
                            type = "string",
                            description =
                                "The name of the speaker if given, else set to NULL"
                        }, 
                        speaker_id = new
                        {
                            type = "string",
                            description =
                                "The ID of the speaker if given, else set to NULL"
                        }, 
                        dialogue = new
                        {
                            type = "string",
                            description = "The next line of dialogue that is going to be said. For example 'What's goin on guys! How are you doing?'"
                        }
                    },
                    required = new[] {"action_type", "speaker_name", "speaker_id", "dialogue"}
                }
            };
            return new dynamic[] {functionDescription};
        }
    }
}