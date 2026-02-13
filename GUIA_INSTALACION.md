# Guia de Instalacion - Documentation Generator AI

## Que necesitas antes de empezar

- **Revit 2025** instalado
- **Visual Studio 2022** (version 17.8 o superior) con el workload ".NET desktop development"
- Una **API key de OpenAI** (empieza con `sk-`)

---

## Paso 1: Configurar la API key

Abre PowerShell y ejecuta:

```powershell
[System.Environment]::SetEnvironmentVariable("OPENAI_API_KEY", "sk-tu-clave-aqui", "User")
```

> Despues de esto, cierra y vuelve a abrir cualquier terminal o aplicacion para que tome efecto.

---

## Paso 2: Compilar el proyecto

1. Abre `DocumentationGeneratorAI.sln` en Visual Studio 2022
2. Compila la solucion con **Ctrl+Shift+B**
3. Al compilar, el archivo `.addin` se copia automaticamente a la carpeta de Revit con la ruta correcta de tu maquina

> Si ves un warning de que no se encontro la carpeta de Revit Addins, copia manualmente el archivo `DocumentationGeneratorAI.addin` desde la carpeta de build (`bin\Debug\net8.0-windows\`) a `C:\ProgramData\Autodesk\Revit\Addins\2025\` y reemplaza `__ASSEMBLY_PATH__` con la ruta completa al DLL compilado.

---

## Paso 3: Abrir Revit y verificar

1. Abre **Revit 2025**
2. Busca la pestana **"Irrelevant"** en el ribbon superior
3. Debe aparecer el boton **"Generate Documentation"**

---

## Paso 4: Probar que funciona

1. Abre un proyecto de Revit (cualquiera sirve para probar)
2. Click en **Generate Documentation**
3. Selecciona tipo de documento, fase, audiencia y nivel de detalle
4. Click en **Generate**
5. Espera a que se genere el documento y revisalo en el panel de preview
6. Click en **Export to Markdown** para guardar

---

## Como saber si algo fallo

Revisa el archivo de log en:

```
%APPDATA%\DocumentationGeneratorAI\logs\plugin.log
```

Para abrirlo rapido, pega esa ruta en el Explorador de Windows.

---

## Checklist rapido

- [ ] La solucion compila sin errores
- [ ] El archivo `.addin` existe en `C:\ProgramData\Autodesk\Revit\Addins\2025\`
- [ ] El plugin aparece en Revit con la pestana "Irrelevant"
- [ ] El boton tiene icono
- [ ] Al hacer click multiples veces solo se abre **una** ventana
- [ ] La generacion funciona con un documento abierto
- [ ] El archivo de log se crea en `%APPDATA%\DocumentationGeneratorAI\logs\`

---

## Modo Demo (sin gastar creditos de OpenAI)

Si quieres probar sin hacer llamadas a la API, configura esta variable de entorno:

```powershell
[System.Environment]::SetEnvironmentVariable("DOCGEN_DEMO_MODE", "true", "User")
```

Esto genera documentos con datos de ejemplo. Para volver al modo normal, cambia el valor a `"false"`.
