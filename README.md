# 🚀 GPT CV Assistant - Modernized AI-Powered Career Platform

[![.NET](https://img.shields.io/badge/.NET-7.0-blue.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](https://github.com/mazharhayyat/gpt-cv-assistant)

A cutting-edge AI-powered interactive CV platform designed for modern recruitment and career development. Built with ASP.NET Core MVC and integrated with multiple AI services including OpenAI and Google Gemini.

## 🌟 Features

### Core Functionality
- **🤖 Dual AI Integration**: Seamlessly integrated with both OpenAI GPT-4 and Google Gemini 2.0
- **💬 Interactive Chat Interface**: Natural language conversation about career achievements
- **🎯 Job Match Agent**: AI-powered job analysis, cover letter generation, and ATS optimization
- **📊 Performance Monitoring**: Built-in metrics and health checks
- **🔒 Enterprise Security**: CSRF protection, input sanitization, and secure session management

### Advanced AI Capabilities
- **📋 Resume Analysis**: Intelligent parsing and highlighting of key skills and experiences
- **✍️ Cover Letter Generation**: Tailored cover letters based on job descriptions
- **🔍 ATS Optimization**: Keyword extraction and resume optimization for ATS systems
- **🎯 Job Matching**: Comprehensive job fit analysis with strengths and gap identification
- **🗣️ Interview Preparation**: AI-powered interview coaching and question practice

### Technical Excellence
- **⚡ High Performance**: Response caching, compression, and optimized data handling
- **🛡️ Robust Error Handling**: Global exception middleware with detailed error categorization
- **📈 Health Monitoring**: Comprehensive health checks for all AI services
- **🎨 Modern UI/UX**: Responsive design with dark/light theme support
- **📱 Mobile-Optimized**: Fully responsive across all device sizes

## 🏗️ Architecture

### Technology Stack
- **Backend**: ASP.NET Core 7.0 MVC
- **Frontend**: Bootstrap 5, jQuery, Modern JavaScript
- **AI Services**: OpenAI GPT-4, Google Gemini 2.0
- **Logging**: Serilog with file and console outputs
- **Caching**: In-memory caching with configurable expiration
- **Security**: HTML Sanitization, CSRF protection, secure headers

### Project StructureGPTCvAssistant/
├── Controllers/          # MVC Controllers
├── Services/            # AI and business logic services
├── Models/              # Data models and DTOs
├── Views/               # Razor views and layouts
├── Configuration/       # Settings and configuration classes
├── Middleware/          # Custom middleware components
├── Constants/           # Application constants
├── Extensions/          # Extension methods and utilities
├── wwwroot/            # Static assets (CSS, JS, images)
└── App_Data/           # CV and data files
## 🚀 Getting Started

### Prerequisites
- .NET 7.0 SDK or later
- Visual Studio 2022 or VS Code
- OpenAI API key (Azure OpenAI or OpenAI)
- Google AI API key (for Gemini)

### Installation

1. **Clone the repository**git clone https://github.com/mazharhayyat/gpt-cv-assistant.git
cd gpt-cv-assistant
2. **Configure API Keys**
   Update `appsettings.json` with your API keys:{
  "OpenAI": {
    "ApiKey": "your-openai-api-key",
    "ApiEndpoint": "your-azure-openai-endpoint"
  },
  "Gemini": {
    "ApiKey": "your-gemini-api-key"
     }
   }
3. **Install Dependencies**dotnet restore
4. **Run the Application**dotnet run
5. **Access the Application**
   Navigate to `https://localhost:7000` in your browser

## 🔧 Configuration

### AI Service Configuration

#### OpenAI Settings{
  "OpenAI": {
    "ApiKey": "your-key",
    "ApiEndpoint": "your-endpoint",
    "ModelName": "gpt-4o",
    "Temperature": 0.7,
    "MaxTokens": 2048,
    "TimeoutSeconds": 120
  }
}
#### Gemini Settings{
  "Gemini": {
    "ApiKey": "your-key",
    "BaseUrl": "https://generativelanguage.googleapis.com/v1beta/",
    "ModelName": "gemini-2.0-flash",
    "Temperature": 0.7,
    "MaxOutputTokens": 2048
  }
}
### Application Features{
  "Application": {
    "Features": {
      "EnableCaching": true,
      "EnableHealthChecks": true,
      "EnableDetailedLogging": true,
      "EnablePerformanceMetrics": true
    }
  }
}
## 🎯 Usage Examples

### Basic Chat Interaction// Ask about technical skills
"What are Mazhar's key technical competencies in AI and cloud architecture?"

// Career summary request
"Summarize Mazhar's career journey as an AI Solutions Architect"

// Project showcase
"Show examples of AI projects Mazhar has delivered"
### Job Match Agent// Analyze job fit
POST /Chat/AnalyzeJob
{
  "jobDescription": "Senior AI Solutions Architect role...",
  "companyName": "Microsoft",
  "targetRole": "AI Solutions Architect"
}

// Generate cover letter
POST /Chat/GenerateCoverLetter
{
  "jobDescription": "...",
  "companyName": "Microsoft"
}
## 📊 Health Monitoring

The application includes comprehensive health checks accessible at `/health`:
{
  "status": "Healthy",
  "checks": [
    {
      "name": "gemini",
      "status": "Healthy",
      "duration": "00:00:01.234"
    },
    {
      "name": "openai",
      "status": "Healthy",
      "duration": "00:00:00.987"
    }
  ]
}
## 🛡️ Security Features

- **Input Sanitization**: All user inputs are sanitized using HtmlSanitizer
- **CSRF Protection**: Built-in anti-forgery token validation
- **Secure Headers**: Comprehensive security headers for production
- **Session Security**: Secure session management with timeout
- **Error Handling**: Sanitized error messages to prevent information leakage

## 🔄 Recent Improvements (v2.1.0)

### Architecture Enhancements
- ✅ Modernized dependency injection and service registration
- ✅ Enhanced configuration validation with data annotations
- ✅ Comprehensive logging with Serilog integration
- ✅ Performance monitoring and metrics collection
- ✅ Robust error handling with categorized exception responses

### AI Service Improvements
- ✅ Dual AI provider support (OpenAI + Gemini)
- ✅ Response caching for improved performance
- ✅ Health checks for AI service availability
- ✅ Enhanced prompt engineering for better responses
- ✅ Timeout and retry handling

### User Experience
- ✅ Modernized chat interface with better styling
- ✅ Enhanced Job Match Agent with multiple features
- ✅ Improved session management and history
- ✅ Better mobile responsiveness
- ✅ Dark/light theme support

### Code Quality
- ✅ Comprehensive error handling and logging
- ✅ Enhanced security measures
- ✅ Performance optimizations
- ✅ Better code organization and documentation
- ✅ Unit test ready architecture

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👤 About Mazhar Hayat

**AI Solutions Architect | .NET Expert | Cloud Innovator**

Based in Abu Dhabi, UAE, Mazhar specializes in:
- 🤖 AI-powered enterprise solutions
- 🔗 Large Language Model (LLM) integration
- 📚 Retrieval-Augmented Generation (RAG) systems
- ☁️ Azure cloud architecture
- 💻 .NET ecosystem development

Connect with Mazhar:
- 💼 [LinkedIn](https://www.linkedin.com/in/mazharhayyat)
- 🌐 [Portfolio](https://mazharhayyat.dev)

## 🙏 Acknowledgments

- OpenAI for the GPT-4 API
- Google for the Gemini AI API
- ASP.NET Core team for the excellent framework
- Bootstrap team for the responsive framework
- All contributors and supporters

---

**Made with ❤️ in Abu Dhabi | Powered by AI | Built for the Future**