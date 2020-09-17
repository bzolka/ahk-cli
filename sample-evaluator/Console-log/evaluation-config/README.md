# Evaluation configuration

The name of the Docker image to execute.

```json
"imageName": "sampleevaluator2"
```

The directory within the container to mount the student's solution.

```json
"solutionInContainer": "/submission"
```

The directory within the container where evaluation results are placed.

```json
"resultInContainer": "/result"
```

Maximum time for evaluation.

```json
"evaluationTimeout": "00:00:30"
```

Optional parameters for container creation, e.g., memory amount. Each item is a key-value pair. The key is a parameter of [Docker Engine API CreateContainer](https://docs.docker.com/engine/api/v1.30/#operation/ContainerCreate) operations. (Supports only string and number values.)

```json
"containerParams":{
    "memory": "268435456"
}
```

Enables the console-based grader and sets a validation code. This code must be included in the messages printed to console by the evaluator application itself. The value must be included in the evaluation container, but must be a secret so that messages cannot be falsified by students.

```json
"consoleMessageGrader": {
    "validationCode": "QWE789"
}
```
