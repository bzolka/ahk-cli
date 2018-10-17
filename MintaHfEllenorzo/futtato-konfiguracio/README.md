# Leírás

Minta konfigurációs fájl a kiértékelés futtatásához.

## Tartalma

Megadja a Docker image nevét, amiben a megoldást futtatni és ellenőrizni szeretnénk.

```json
    "imageName": "aut/mintahf1"
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

Az eredmény könyvtárban elvárt TRX fájl a pontozáshoz.

```json
    "trxFileName": "testresult.trx"
```

A kiértékelések összegzését tartalmazó fájl neve. Egy-egy sora egy hallgató által beadott megoldás értékelése.

```json
    "resultFileName": "eremenyek.csv
```