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

Az eredmény könyvtárban elvárt TRX fájl a pontozáshoz.

```json
    "trxFileName": "testresult.trx"
```

A kiértékelések összegzését tartalmazó fájl neve. Egy-egy sora egy hallgató által beadott megoldás értékelése.

```json
    "resultFileName": "eremenyek.csv
```