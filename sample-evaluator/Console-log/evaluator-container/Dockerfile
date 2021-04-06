# Base image
FROM mcr.microsoft.com/powershell:alpine-3.12

# Copy the evaluator application
COPY ["src/Evaluate-Homework.ps1", "/"]
# Entry point starts the evaluation
CMD pwsh -File /Evaluate-Homework.ps1