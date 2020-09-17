# Base image
FROM ubuntu:16.04
# Install required tools, such as PowerShell
RUN apt-get update \
    && apt-get install -y wget apt-transport-https\
    # https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-linux?view=powershell-6
    && wget -q https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y powershell

# Copy the evaluator application
COPY ["src/Evaluate-Homework.ps1", "/"]
# Entry point starts the evaluation
CMD pwsh -File /Evaluate-Homework.ps1