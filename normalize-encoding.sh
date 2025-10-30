#!/bin/bash
# Script para re-normalizar todos los archivos a UTF-8

echo "🔧 Normalizando archivos a UTF-8..."

# Encuentra todos los archivos .cs, .cshtml, .json, etc.
find . -type f \( -name "*.cs" -o -name "*.cshtml" -o -name "*.json" -o -name "*.sql" \) \
  ! -path "*/bin/*" \
  ! -path "*/obj/*" \
  ! -path "*/.git/*" \
  -exec sh -c '
    for file; do
      # Detectar encoding actual
      encoding=$(file -bi "$file" | sed -e "s/.*[ ]charset=//")
      
      if [ "$encoding" != "utf-8" ] && [ "$encoding" != "us-ascii" ]; then
        echo "Convirtiendo: $file (desde $encoding)"
        # Convertir a UTF-8
        iconv -f "$encoding" -t UTF-8 "$file" > "$file.tmp" && mv "$file.tmp" "$file"
      fi
    done
  ' sh {} +

echo "✅ Normalización completada"
