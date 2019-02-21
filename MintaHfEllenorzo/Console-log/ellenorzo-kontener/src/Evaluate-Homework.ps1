$MegoldasFajl = "/megoldas/megoldas.txt"

# Konyolra barmilyen uzenet irhato
Write-Output "Minta ellenorzo script"

# Elso teszt: ellenorzi, hogy letezik-e a fajl
if (Test-Path $MegoldasFajl) {
    # Sikeres tesztrol uzenet: kotelezo prefix # validacios kod # utasitas: teszteredmeny kovetkezik # feladat neve # siker=passed
    Write-Output "###ahk#QWE789#testresult#1. feladat: fajl letezik#passed"
}
else {
    # Sikertelen tesztrol uzenet: kotelezo prefix # validacios kod # utasitas: teszteredmeny kovetkezik # feladat neve # siker=failed # opcionalis hibauzenet
    Write-Output "###ahk#QWE789#testresult#1. feladat: fajl letezik#failed#A fajl nem letezik"
}

# Masodik teszt: fajl tartalom ellenorzes
$MegoldasSzoveg = Get-Content $MegoldasFajl
if ($MegoldasSzoveg -like "*42*") {
    Write-Output "###ahk#QWE789#testresult#2. feladat: jo valasz#passed"
}
else {
    Write-Output "###ahk#QWE789#testresult#2. feladat: jo valasz#failed#Hibas ertek"
}