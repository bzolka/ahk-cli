# Sample evaluator using .NET Core unit tests

A sample evaluator using .NET Core unit tests. See the Dockerfile and the source code of the evaluator [here](evaluator-container).

Build the Docker image: `docker build -t sampleevaluator-trx evaluator-container`

Execute: `dotnet ahk.dll eval docker trx --trxfile testresult.trx --image sampleevaluator-trx --mount-path /submission --artifact-path /result -s ./../sample-input -o "./evaluation-output-{date}"`
