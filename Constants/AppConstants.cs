namespace GPTCvAssistant.Constants
{
    /// <summary>
    /// Application-wide constants
    /// </summary>
    public static class AppConstants
    {
        /// <summary>
        /// Session keys
        /// </summary>
        public static class SessionKeys
        {
            public const string ChatHistory = "ChatHistory";
            public const string InterviewStep = "InterviewStep";
            public const string UsageMetrics = "UsageMetrics";
        }

        /// <summary>
        /// File paths and names
        /// </summary>
        public static class FilePaths
        {
            public const string CVDataPath = "App_Data";
            public const string CVFileName = "ExtractedCV.txt";
            public const string TranscriptFileName = "CV-GPT-Transcript.txt";
        }

        /// <summary>
        /// AI prompt templates
        /// </summary>
        public static class PromptTemplates
        {
            public const string CareerSummary = @"
                Act as a Career Narrator.
                Return valid HTML only (<h3>, <p>, <ul>, <li>, <strong>).
                Summarize Mazhar Hayat's career as an AI Solutions Architect in Abu Dhabi.
                Focus on leadership, AI solutions, .NET, LLM, RAG, and Azure expertise.
            ";

            public const string SkillsHighlight = @"
                Act as a Technical Skills Highlighter.
                Return valid HTML only (<h3>, <p>, <ul>, <li>, <strong>).
                Highlight Mazhar Hayat's strongest technical skills:
                - .NET ecosystem
                - Large Language Models (LLM)
                - Retrieval-Augmented Generation (RAG)
                - Azure Cloud
                - AI-powered enterprise solutions
            ";
        }

        /// <summary>
        /// HTML sanitizer allowed elements
        /// </summary>
        public static class AllowedHtmlElements
        {
            public static readonly string[] Tags = { "h1", "h2", "h3", "ul", "li", "strong", "em", "p", "br", "div", "span" };
            public static readonly string[] Attributes = { "class", "style" };
        }

        /// <summary>
        /// Default suggestions for user prompts
        /// </summary>
        public static class DefaultSuggestions
        {
            public static readonly List<string> Prompts = new()
            {
                "Summarize Mazhar's career as an AI Solutions Architect.",
                "Highlight Mazhar's expertise with .NET, LLM, RAG, and Azure.",
                "What leadership and team roles has Mazhar taken in Abu Dhabi?",
                "Explain Mazhar's experience in building AI-powered enterprise solutions.",
                "Generate a quick overview of Mazhar's projects in data and AI.",
                "How does Mazhar apply RAG techniques in real-world systems?",
                "What makes Mazhar a strong fit for AI architect roles?",
                "?? Analyze a job description for match and generate tailored materials"
            };
        }
    }
}