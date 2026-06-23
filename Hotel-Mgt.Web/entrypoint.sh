#!/bin/sh
# Ensure directories exist on the persistent volume
mkdir -p /app/data/uploads

# Create symlink for uploads to wwwroot if it doesn't already exist
if [ ! -L /app/wwwroot/uploads ]; then
  # If a directory already exists at /app/wwwroot/uploads and isn't a symlink, copy its contents first
  if [ -d /app/wwwroot/uploads ]; then
    cp -r /app/wwwroot/uploads/* /app/data/uploads/ 2>/dev/null || true
    rm -rf /app/wwwroot/uploads
  fi
  ln -s /app/data/uploads /app/wwwroot/uploads
fi

# Run the app
exec dotnet HotelMgt.Web.dll
