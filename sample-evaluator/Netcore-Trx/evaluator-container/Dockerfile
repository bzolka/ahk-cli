# Image for building the evaluator .NET Core application
FROM microsoft/dotnet:2.1.402-sdk-bionic as builder
# Copy the source code
COPY src /src/
# Build the evaluator application
WORKDIR /src
RUN dotnet publish -o /app -c Release --runtime ubuntu.16.04-x64
# This container terminates here; the results of the build are used in the next stage to build the final image



# Base image for the evaluator application containing the runtime for .NET Core
FROM microsoft/dotnet:2.1.402-sdk-bionic
# Copy the evaluator application built above into this container
COPY --from=builder /app .
# Entry point of the container: executes the unit tests and outputs a TRX file as result.
CMD dotnet vstest SampleEval.dll --logger:"trx;LogFileName=testresult.trx" --ResultsDirectory:/result
