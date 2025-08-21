# ?? Testing Guide for Advanced Career Features

## Quick Start Testing

### 1. Launch the Application
```bash
dotnet run
```
Navigate to `https://localhost:7000`

### 2. Access Advanced Tools
Click the **?? Advanced Tools** button in the action bar to open the advanced features modal.

## Feature Testing Scenarios

### ?? Skills Gap Analysis
1. Click "Start Assessment" on the Skills Gap Analysis card
2. Fill in the form:
   - **Target Role**: "Senior AI Architect"
   - **Industry**: "Technology"
   - **Experience Level**: "Senior Level (8+ years)"
   - **Current Skills**: "C#, .NET, Azure, Machine Learning, Python, SQL, Leadership"
3. Click "Analyze Skills Gap"
4. Verify the response appears with purple styling and detailed analysis

### ?? Market Intelligence
1. Click "Analyze Market" on the Market Intelligence card
2. Fill in the form:
   - **Role**: "AI Solutions Architect"
   - **Industry**: "Technology"
   - **Location**: "UAE"
3. Click "Analyze Market"
4. Verify the response appears with green styling and market insights

### ?? Video Interview Practice
1. Click "Practice Now" on the Video Interview Prep card
2. Allow camera access when prompted
3. Verify video feed appears in the modal
4. Observe the AI-generated interview question
5. Type an answer and click "Submit Answer"
6. Verify the AI provides feedback and a follow-up question

### ?? Personal Brand Builder
1. Click "Build Brand" on the Personal Brand Builder card
2. Fill in the form:
   - **Current Role**: "AI Solutions Architect"
   - **Target Role**: "Head of AI"
   - **Industry**: "Technology"
3. Click "Generate Brand Strategy"
4. Verify the response appears with yellow styling and brand strategy sections

### ?? Network Builder
1. Click "Build Network" on the Network Builder card
2. Fill in the form:
   - **Target Role**: "AI Solutions Architect"
   - **Industry**: "Technology"
   - **Location**: "UAE"
3. Click "Create Strategy"
4. Verify the response appears with gray styling and networking recommendations

### ??? Career Path Analysis
1. Click "Explore Paths" on the Career Path Analysis card
2. Fill in the form:
   - **Current Role**: "AI Solutions Architect"
   - **Years of Experience**: "8"
   - **Career Interests**: "Leadership roles, technical innovation, team management"
3. Click "Analyze Career Paths"
4. Verify the response appears with dark styling and career path recommendations

## Expected Results

### Visual Verification
- Each feature should have distinct color-coded styling
- Modals should open and close smoothly
- Forms should validate required fields
- Loading states should appear during AI processing
- Success toasts should confirm completion

### Functional Verification
- All API endpoints should respond successfully
- AI-generated content should be relevant and well-formatted
- Chat history should preserve feature responses
- Special intent labels should appear on responses
- Copy and pin functionality should work on all responses

## Common Issues and Solutions

### 1. AI Service Not Responding
**Symptoms**: Loading spinner never stops, error messages
**Solutions**: 
- Check API keys in `appsettings.json`
- Verify internet connectivity
- Check console for error messages

### 2. Modal Not Opening
**Symptoms**: Button clicks have no effect
**Solutions**:
- Check browser console for JavaScript errors
- Ensure Bootstrap JS is loaded
- Verify modal IDs match function calls

### 3. Styling Issues
**Symptoms**: Cards don't have special colors
**Solutions**:
- Verify CSS file is loaded
- Check if intent labels are correctly assigned
- Ensure JavaScript observers are working

### 4. Form Validation Errors
**Symptoms**: Required field warnings, submission failures
**Solutions**:
- Fill all required fields (marked with *)
- Ensure text inputs are not empty
- Check network requests in browser dev tools

## Performance Testing

### Load Testing
1. Rapidly click multiple feature buttons
2. Submit multiple requests simultaneously
3. Verify system remains responsive
4. Check for memory leaks in browser

### Mobile Testing
1. Open application on mobile device
2. Test all modals for responsiveness
3. Verify touch interactions work properly
4. Check form inputs on mobile keyboards

## Debug Mode
Enable detailed logging by setting in `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "GPTCvAssistant": "Debug"
    }
  }
}
```

## Browser Compatibility
Tested on:
- ? Chrome 120+
- ? Firefox 115+
- ? Edge 120+
- ? Safari 16+

## API Testing with Postman

### Skills Gap Analysis
```
POST https://localhost:7000/AdvancedCareer/AnalyzeSkillsGap
Content-Type: application/json

{
  "targetRole": "Senior AI Architect",
  "currentSkills": ["C#", ".NET", "Azure", "Machine Learning"],
  "industry": "Technology",
  "experienceLevel": "Senior"
}
```

### Market Intelligence
```
POST https://localhost:7000/AdvancedCareer/GetCareerTrends
Content-Type: application/json

{
  "role": "AI Solutions Architect",
  "industry": "Technology",
  "location": "UAE"
}
```

## Success Criteria
- ? All 6 advanced features are accessible
- ? Modals open and close properly
- ? Forms validate and submit correctly
- ? AI responses are generated and displayed
- ? Special styling is applied to responses
- ? Mobile experience is functional
- ? No console errors during normal usage
- ? Performance remains smooth under load

---

**?? Congratulations! Your AI-Powered Career Profile now includes comprehensive advanced features that provide users with a complete career development toolkit.**