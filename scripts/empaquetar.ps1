# Genera SAGE_API.zip con lo que está en el último commit: sin bin, obj, .vs ni esta guía (.gitattributes).
$raiz = Split-Path $PSScriptRoot -Parent
git -C $raiz archive --format=zip --prefix=SAGE_API/ -o (Join-Path $raiz 'SAGE_API.zip') HEAD
Write-Host "Generado: $(Join-Path $raiz 'SAGE_API.zip')"
