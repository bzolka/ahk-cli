# Leírás

Minta kiértékelő egy házi feladathoz.

Elemei

- [Docker image](ellenorzo-kontener), amely a kiértelésért felelős
- [Kiértékelő alkalmazás](ellenorzo-kontener/src), amely a megoldás ellenőrzését végzi powershell script formájában. Az előbbi image tartalmazza.
- [Futtató konfiguráció](futtato-konfiguracio), amely a parancssori alkalmazást vezérli
- [Minta hallgatói megoldások](../minta-hallgatoi-megoldasok) a teszteléshez

## Futtatás

```
dotnet AHK.dll -k "./futtato-konfiguracio" -m "./../minta-hallgatoi-megoldasok" -e "./futas-eredmenye-{datum}"
```
