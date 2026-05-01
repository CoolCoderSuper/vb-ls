#!/usr/bin/env sh
set -e

url=$(sed -n 's/.*"url"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' roslyn.json)
point=$(sed -n 's/.*"point"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' roslyn.json)
rm -rf roslyn
git init roslyn
git -C roslyn remote add origin "$url"
git -C roslyn fetch --depth 1 origin "$point"
git -C roslyn checkout FETCH_HEAD
for patch in vb-ls/patches/*.patch; do [ -e "$patch" ] && git -C roslyn apply "../$patch"; done
rm -rf roslyn/.git
