namespace GPTCvAssistant.Constants
{
    /// <summary>
    /// Application-wide constants for the modernized GPT CV Assistant
    /// </summary>
    public static class AppConstants
    {
        /// <summary>
        /// Session keys for managing user state
        /// </summary>
        public static class SessionKeys
        {
            public const string ChatHistory = "ChatHistory";
            public const string InterviewStep = "InterviewStep";
            public const string UsageMetrics = "UsageMetrics";
            public const string UserPreferences = "UserPreferences";
            public const string LastActivity = "LastActivity";
        }

        /// <summary>
        /// File paths and names for CV and data management
        /// </summary>
        public static class FilePaths
        {
            public const string CVDataPath = "App_Data";
            public const string CVFileName = "ExtractedCV.txt";
            public const string TranscriptFileName = "CV-GPT-Transcript.txt";
            public const string LogsPath = "logs";
            public const string CachePath = "cache";
        }

        /// <summary>
        /// Enhanced AI prompt templates for consistent responses
        /// </summary>
        public static class PromptTemplates
        {
            public const string CareerSummary = @"
                Act as a Career Narrator and Professional Storyteller.
                
                Task: Create a compelling career summary for Mazhar Hayat highlighting his journey as an AI Solutions Architect.
                
                Focus Areas:
                - Leadership roles and team management experience in Abu Dhabi
                - AI and machine learning solution architecture
                - .NET ecosystem expertise and enterprise development
                - Large Language Models (LLM) implementation and optimization
                - Retrieval-Augmented Generation (RAG) system design
                - Azure cloud architecture and DevOps practices
                - Cross-functional collaboration and stakeholder management
                
                Style: Professional, engaging, achievement-focused
                Format: Return valid HTML only (<h3>, <p>, <ul>, <li>, <strong>)
                Length: Comprehensive but concise (300-500 words)
                
                Important: Do not use emoji characters in your response.
            ";

            public const string SkillsHighlight = @"
                Act as a Technical Skills Showcaser and Competency Analyst.
                
                Task: Highlight Mazhar Hayat's strongest technical competencies in a structured format.
                
                Key Technical Areas:
                - .NET Framework & .NET Core ecosystem
                - C# programming and advanced language features
                - Large Language Models (LLM) integration and fine-tuning
                - Retrieval-Augmented Generation (RAG) architecture
                - Azure cloud services (AI Services, Cognitive Services, App Services)
                - Machine Learning and AI model deployment
                - Microservices architecture and API design
                - DevOps practices and CI/CD pipelines
                - Database design and optimization
                - Enterprise software architecture patterns
                
                Format: Return valid HTML only (<h3>, <p>, <ul>, <li>, <strong>)
                Structure: Organize by categories with specific examples
                
                Important: Do not use emoji characters in your response.
            ";

            public const string ProjectShowcase = @"
                Act as a Project Portfolio Curator.
                
                Task: Showcase Mazhar Hayat's most impactful projects and achievements.
                
                Focus on:
                - AI-powered enterprise solutions and their business impact
                - RAG system implementations and performance improvements
                - Cloud migration and modernization projects
                - Team leadership and cross-functional collaboration
                - Innovation and technical problem-solving
                
                Format: Return valid HTML only (<h3>, <p>, <ul>, <li>, <strong>)
                Style: Results-oriented with quantifiable achievements
                
                Important: Do not use emoji characters in your response.
            ";

            public const string InterviewPrep = @"
                Act as an AI Solutions Architect Interview Coach.
                
                Task: Prepare comprehensive interview responses based on Mazhar Hayat's background.
                
                Cover:
                - Technical deep-dive questions on AI/ML architecture
                - Leadership and team management scenarios
                - Problem-solving and system design challenges
                - Azure cloud architecture decisions
                - RAG and LLM implementation strategies
                
                Format: Return valid HTML only (<h3>, <p>, <ul>, <li>, <strong>)
                Style: Structured, detailed, and confident
                
                Important: Do not use emoji characters in your response.
            ";
        }

        /// <summary>
        /// HTML sanitizer configuration for security
        /// </summary>
        public static class AllowedHtmlElements
        {
            public static readonly string[] Tags = { 
                "h1", "h2", "h3", "h4", "h5", "h6", 
                "ul", "ol", "li", 
                "strong", "em", "b", "i", 
                "p", "br", "div", "span", 
                "blockquote", "code", "pre",
                "table", "thead", "tbody", "tr", "th", "td"
            };
            
            public static readonly string[] Attributes = { 
                "class", "style", "id", "data-*"
            };
        }

        /// <summary>
        /// Enhanced default suggestions for user prompts
        /// </summary>
        public static class DefaultSuggestions
        {
            public static readonly List<string> Prompts = new()
            {
                "Summarize Mazhar's career as an AI Solutions Architect",
                "Highlight Mazhar's expertise with .NET, LLM, RAG, and Azure",
                "What leadership and team roles has Mazhar taken in Abu Dhabi?",
                "Explain Mazhar's experience in building AI-powered enterprise solutions",
                "Generate a quick overview of Mazhar's projects in data and AI",
                "How does Mazhar apply RAG techniques in real-world systems?",
                "What makes Mazhar a strong fit for AI architect roles?",
                "Describe Mazhar's approach to cloud architecture and DevOps",
                "Show examples of Mazhar's cross-functional collaboration",
                "What are Mazhar's key achievements in AI solution delivery?"
            };

            public static readonly List<string> InterviewQuestions = new()
            {
                "Tell me about your experience with Large Language Models",
                "How do you design RAG systems for enterprise applications?",
                "Describe your approach to AI solution architecture",
                "What's your experience with Azure AI services?",
                "How do you handle AI model deployment and monitoring?",
                "Tell me about a challenging AI project you led",
                "How do you ensure AI solutions are scalable and reliable?",
                "Describe your experience with .NET in AI applications"
            };
        }

        /// <summary>
        /// API and service configuration constants
        /// </summary>
        public static class ServiceConstants
        {
            public const int DefaultTimeoutSeconds = 120;
            public const int MaxRetryAttempts = 3;
            public const int CacheExpirationMinutes = 10;
            public const string UserAgent = "GPTCvAssistant/2.1.0";
        }

        /// <summary>
        /// UI and UX configuration constants
        /// </summary>
        public static class UIConstants
        {
            public const int MaxChatHistoryItems = 50;
            public const int SearchResultsLimit = 20;
            public const int AutoSaveIntervalSeconds = 30;
            public const string DateTimeFormat = "MMM dd, yyyy 'at' h:mm tt";
        }

        /// <summary>
        /// Validation and security constants
        /// </summary>
        public static class ValidationConstants
        {
            public const int MaxQuestionLength = 1000;
            public const int MaxFileSize = 10485760; // 10MB
            public const int SessionTimeoutMinutes = 60;
            public static readonly string[] AllowedFileExtensions = { ".txt", ".pdf", ".docx" };
        }
    }
}