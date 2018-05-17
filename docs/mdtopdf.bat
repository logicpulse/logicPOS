@ECHO OFF
REM install with
REM npm install -g markdown-pdf
CLS
cd docs/
markdown-pdf index.md
cd ..
