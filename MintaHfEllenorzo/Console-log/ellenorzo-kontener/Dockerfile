# Ez a kiindulasi image.
FROM ubuntu:16.04
# Telepitjuk a szukseges komponenseket, pl. Powershell
RUN apt-get update \
    && apt-get install -y wget apt-transport-https\
    # https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-linux?view=powershell-6
    && wget -q https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y powershell

# Bemasoljuk az ellenorzo kodot.
COPY ["src/Evaluate-Homework.ps1", "/"]
# A belépési pont, elindítja az ellenőrzést.
CMD pwsh -File /Evaluate-Homework.ps1