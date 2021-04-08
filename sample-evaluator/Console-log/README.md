# Sample evaluator using PowerShell

A sample evaluator using a PowerShell script. See the Dockerfile and the source code of the evaluator [here](evaluator-container).

Build the Docker image: `docker build -t sampleevaluator-cm evaluator-container`

Execute the evaluation: `dotnet ahk.dll eval docker consolemessage --validationCode Valid55Code --image sampleevaluator-cm --mount-path /submission --artifact-path /result --container-env WEBSERVER=web --service-container nginx --service-container-name web -s ./../sample-input -o "./evaluation-output-{date}"`
