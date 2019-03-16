$MegoldasFajl = "/megoldas/megoldas.txt"
$imscFajl = "/megoldas/imsc.txt"


# Konzolra barmilyen uzenet irhato
Write-Output "Minta ellenorzo script: futas kezdete"




# **** Elso feladatcsoport
# A feladatcsoporton belul az egyes tesztek osszeadodnak, abbol szamitodik ki a feladatcsoport reszeredmenye
# Sikeres "passed" teszt: 1 pont
# Sikertelen "failed" teszt: 0 pont
# Tetszoleges pontszam is adhato szam formajaban

# Elso teszt: ellenorzi, hogy letezik-e a fajl
if (Test-Path $MegoldasFajl) {
    # Sikeres tesztrol uzenet: kotelezo prefix # validacios kod # utasitas: teszteredmeny kovetkezik # feladat neve # siker=passed
    Write-Output "###ahk#Valid55Code#testresult#1. feladat#passed"
}
else {
    # Sikertelen tesztrol uzenet: kotelezo prefix # validacios kod # utasitas: teszteredmeny kovetkezik # feladat neve # siker=failed # opcionalis hibauzenet
    Write-Output "###ahk#Valid55Code#testresult#1. feladat#failed#A fajl nem letezik"
}

# Masodik teszt: fajl tartalom ellenorzes
$MegoldasSzoveg = Get-Content $MegoldasFajl
if ($MegoldasSzoveg -like "*42*") {
    # 2 pontot er a helyes megoldas
    Write-Output "###ahk#Valid55Code#testresult#2. feladat#2#feladat ok"
}
elseif ($MegoldasSzoveg -like "*84*") {
    # A megoldast nem lehet automatikusan kiertelni, "inconclusive" eredmennyel jelezheto, hogy manualis ellenorzes szukseges
    Write-Output "###ahk#Valid55Code#testresult#2. feladat#inconclusive#A megoldast kezzel ellenorizni szukseges"
}
else {
    # A hibauzenete lehet tobb soros is, ilyenkor a sortorest a \ karakterrel jelezzuk (a `n karakter alabb a Powershell sorteres karaktere)
    Write-Output "###ahk#Valid55Code#testresult#2. feladat#0#Hibas ertek\`nLehet tobb soros\`naz uzenet"
}




# **** Masodik feladatcsoport - iMsc feladatok
# A teszt neveben a @ elotti resz a feladatcsoport neve (imsc@3. feladat)
# A @ opcionalis, ha nincs a feladatcsoportnak neve, nem kell megadni

if (Test-Path $imscFajl) {
    if ((Get-Content $imscFajl) -like "*84*") {
        Write-Output "###ahk#Valid55Code#testresult#imsc@3. feladat#passed#iMsc feladat ok"
    }
    else {
        Write-Output "###ahk#Valid55Code#testresult#imsc@3. feladat#failed#Hibas megoldas"
    }
}