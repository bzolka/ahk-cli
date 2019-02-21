# Ez a teszt alkalmazást lefordito kontener
FROM microsoft/dotnet:2.1.402-sdk-bionic as builder
# Atmasoljuk a forraskodot
COPY src /src/
# Leforditjuk az alkalmazast
WORKDIR /src
RUN dotnet publish -o /app -c Release --runtime ubuntu.16.04-x64
# Ez a kontener meg is szunik, nem lesz a vegso eredmeny resze, csak ideiglenesen letezik



# Ez a kiindulasi image. Tartalmazza a teszthez a futtato konyezetet (dotnet CLI)
FROM microsoft/dotnet:2.1.402-sdk-bionic
# Atmasoljuk a fenti ideiglenes kontenerbol a forditas eredmenyet; igy kerul a tesztelo alkalmazas az image-be.
COPY --from=builder /app .
# Teszt futtatasa: ez a parancs a belepesi pont a kontererbe, ez vegzi a lenyegi munkat. Kimenete egy TRX fajl.
CMD dotnet vstest AUT.MintaHf1.dll --logger:"trx;LogFileName=testresult.trx" --ResultsDirectory:/eredmeny \