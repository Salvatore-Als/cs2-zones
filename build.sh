#!/bin/bash

set -e 

dotnet build --configuration Release

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PLUGINS_DIR="$SCRIPT_DIR/release/plugins"
SHARED_DIR="$SCRIPT_DIR/release/shared"

mkdir -p "$PLUGINS_DIR/CS2Zones"
mkdir -p "$PLUGINS_DIR/CS2ZonesExample"
mkdir -p "$SHARED_DIR/CS2ZonesAPI"

echo "📦 Copying release files..."

echo "  → Copying CS2Zones..."
cp -f "$SCRIPT_DIR/CS2Zones/bin/Release/net8.0/CS2Zones.dll" "$PLUGINS_DIR/CS2Zones/"
cp -f "$SCRIPT_DIR/CS2Zones/bin/Release/net8.0/CS2Zones.deps.json" "$PLUGINS_DIR/CS2Zones/" 2>/dev/null || true
cp -f "$SCRIPT_DIR/CS2Zones/bin/Release/net8.0/CS2Zones.pdb" "$PLUGINS_DIR/CS2Zones/" 2>/dev/null || true

echo "  → Copying CS2ZonesExample..."
cp -f "$SCRIPT_DIR/CS2ZonesExample/bin/Release/net8.0/CS2ZonesExample.dll" "$PLUGINS_DIR/CS2ZonesExample/"
cp -f "$SCRIPT_DIR/CS2ZonesExample/bin/Release/net8.0/CS2ZonesExample.deps.json" "$PLUGINS_DIR/CS2ZonesExample/" 2>/dev/null || true
cp -f "$SCRIPT_DIR/CS2ZonesExample/bin/Release/net8.0/CS2ZonesExample.pdb" "$PLUGINS_DIR/CS2ZonesExample/" 2>/dev/null || true

echo "  → Copying CS2ZonesAPI..."
cp -f "$SCRIPT_DIR/CS2ZonesAPI/bin/Release/net8.0/CS2ZonesAPI.dll" "$SHARED_DIR/CS2ZonesAPI/"
cp -f "$SCRIPT_DIR/CS2ZonesAPI/bin/Release/net8.0/CS2ZonesAPI.deps.json" "$SHARED_DIR/CS2ZonesAPI/" 2>/dev/null || true
cp -f "$SCRIPT_DIR/CS2ZonesAPI/bin/Release/net8.0/CS2ZonesAPI.pdb" "$SHARED_DIR/CS2ZonesAPI/" 2>/dev/null || true

echo "✅ Build and copy completed successfully !"

