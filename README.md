# AI-Powered Career Profile Assistant

An innovative, interactive CV platform that transforms traditional resume presentation into an engaging AI-driven conversation experience. Built for modern recruitment, this application allows recruiters and employers to explore candidate information through natural language interactions.

## ?? Features

### Core Capabilities
- **Interactive Chat Interface**: Natural conversation with AI about candidate experience
- **Multi-AI Support**: Integration with both OpenAI and Google Gemini APIs
- **Job Matching Agent**: Advanced job description analysis and matching
- **ATS Optimization**: Keyword extraction and resume optimization for Applicant Tracking Systems
- **Cover Letter Generation**: AI-powered, tailored cover letters for specific roles
- **Interview Simulation**: Mock interview sessions with AI feedback
- **Document Generation**: Customized CV variants and LinkedIn profiles

### Technical Features
- **Real-time Processing**: Instant AI responses with typing indicators
- **Session Management**: Persistent chat history and user preferences
- **HTML Sanitization**: Secure content rendering with XSS protection
- **Export Functionality**: Download conversation transcripts
- **Responsive Design**: Mobile-first UI with Bootstrap 5
- **Voice Input**: Speech-to-text integration
- **Search & Filter**: History search and content filtering
- **Theme Support**: Light/dark mode toggle

## ?? Technology Stack

- **Backend**: ASP.NET Core 7.0 (C#)
- **Frontend**: Razor Pages, Bootstrap 5, jQuery
- **AI Services**: OpenAI GPT-4, Google Gemini
- **Session Storage**: In-memory caching
- **Security**: HTML Sanitizer (HtmlAgilityPack)
- **Styling**: CSS3 with custom animations
- **Icons**: Font Awesome 6

## ?? Project Structure

```
GPTCvAssistant/
??? Configuration/          # Settings and configuration classes
??? Constants/             # Application constants and enums
??? Controllers/           # MVC controllers
??? Extensions/            # Extension methods
??? Middleware/            # Custom middleware
??? Models/               # Data models and DTOs
??? Services/             # Business logic and AI services
?   ??? Interfaces/       # Service contracts
?   ??? Implementation/   # Service implementations
??? Views/                # Razor views and templates
??? wwwroot/             # Static files (CSS, JS, images)
```

## ?? Configuration

### Required API Keys

1. **OpenAI/Azure OpenAI**:
   ```json
   {
     "OpenAI": {
       "ApiKey": "your-openai-api-key",
       "ApiEndpoint": "your-endpoint-url",
       "ModelName": "gpt-4o"
     }
   }
   ```

2. **Google Gemini**:
   ```json
   {
     "Gemini": {
       "ApiKey": "your-gemini-api-key",
       "BaseUrl": "https://generativelanguage.googleapis.com/v1beta/",
       "ModelName": "gemini-2.0-flash"
     }
   }
   ```

### CV Data Setup

Place the candidate's CV content in:
```
App_Data/ExtractedCV.txt
```

## ????? Getting Started

### Prerequisites
- .NET 7.0 SDK or later
- Valid OpenAI and/or Gemini API keys
- Web browser with JavaScript enabled

### Installation

1. **Clone the repository**:
   ```bash
   git clone [repository-url]
   cd GPTCvAssistant
   ```

2. **Configure API keys** in `appsettings.json`

3. **Add CV content** to `App_Data/ExtractedCV.txt`

4. **Install dependencies**:
   ```bash
   dotnet restore
   ```

5. **Run the application**:
   ```bash
   dotnet run
   ```

6. **Open browser** and navigate to `https://localhost:7000`

## ?? Usage Examples

### For Recruiters
- Ask about specific technical skills: *"What's Mazhar's experience with Azure?"*
- Explore project history: *"Tell me about his AI projects"*
- Assess cultural fit: *"What leadership roles has he held?"*

### Job Matching
- Paste job description for instant analysis
- Get match percentage and gap analysis
- Generate tailored cover letters
- Extract ATS-optimized keywords

### Interview Preparation
- Start mock interview sessions
- Get AI-generated questions for specific roles
- Practice responses with instant feedback

## ?? Architecture Highlights

### Dependency Injection
- Clean separation of concerns
- Interface-based service contracts
- Scoped service lifetimes for session management

### Error Handling
- Global exception middleware
- Structured logging with Serilog
- Graceful degradation for API failures

### Security
- HTML sanitization for all AI responses
- Session-based state management
- HTTPS enforcement in production

### Performance
- Async/await patterns throughout
- HTTP client factory for API calls
- Session-based caching for chat history

## ?? Deployment

### Development
```bash
dotnet run --environment Development
```

### Production
```bash
dotnet publish -c Release
# Deploy to your preferred hosting platform
```

### Docker (Optional)
```dockerfile
# Dockerfile example
FROM mcr.microsoft.com/dotnet/aspnet:7.0
COPY . /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "GPTCvAssistant.dll"]
```

## ?? Roadmap

- [ ] Integration with LinkedIn API
- [ ] PDF resume generation
- [ ] Multi-language support
- [ ] Analytics dashboard
- [ ] Email integration for sharing
- [ ] Candidate profile templates
- [ ] Advanced search capabilities
- [ ] Mobile app companion

## ?? Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## ?? License

This project is licensed under the MIT License - see the LICENSE file for details.

## ?? Support

For issues and questions:
- Create an issue in the repository
- Contact: [your-email@domain.com]

## ?? Keywords

AI CV, Interactive Resume, Job Matching, ATS Optimization, Recruitment Technology, AI Assistant, Career Profile, Modern Hiring, Conversational Interface, Candidate Experience

---

*Built with ?? for the future of recruitment*