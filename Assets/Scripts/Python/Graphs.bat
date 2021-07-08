@echo off
echo "Zeichne die Diagramme..."
python ReadCSV_Altair.py
echo "Fertig! Die Graphen liegen im StreamingAssets-Ordner im Unterordner Graphs."
pause
exit