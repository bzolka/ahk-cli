# Leírás

Minta konfigurációs fájl a kiértékelés futtatásához.

## Tartalma

Megadja a Docker image nevét, amiben a megoldást futtatni és ellenőrizni szeretnénk.

```json
    "imageName": "aut/mintahf2"
```

Megadja, hogy a konténerbe milyen nevű mappába kerüljön a hallgató megoldása.

```json
    "solutionInContainer": "/megoldas"
```

Megadja, hogy a konténerben milyen nevű mappába készül el a kiértékelés eredménye.

```json
    "resultInContainer": "/eredmeny"
```

Megadja a maximális futási időt.

```json
    "evaluationTimeout": "00:00:30"
```

Opcionális paraméterek a konténer létrehozásához. Tipikusan pl. használható memória mérete. Minden elem egy kulcs-érték pár, a kulcs a tulajdonság neve. A tulajdonság neve egy a [Docker Engine API CreateContainer](https://docs.docker.com/engine/api/v1.30/#operation/ContainerCreate) hívásának paraméterei közül. (Csak skalár értékekre és string-ből a szükséges típusra konvertálható tulajdonságokra működik.)

```json
    "containerParams":{
            "memory": "268435456"
        }
```

Konzolra írt üzenetektől várja a tesztek eredményét. A konzolra a konténerben futó alkalmazás/szript ír. Az üzenetek validálásához az üzenetben szerepelnie kell az itt megadott kódnak (hogy a hallgató nem írhasson olyan kódot, ami fals eredményt naplóz ki). Ezt a kódot nem ismerheti meg a hallgató, de a konténerben futó alkalmazásnak ismernie kell.

```json
    "consoleMessageGrader": {
        "validationCode": "QWE789"
    }
```
