# Programm zur Erstellung von Liniendiagrammen auf Basis von in Unity generierten csv-Dateien
# Autor: Verena Kauth (im Rahmen der Bachelorarbeit im Studiengang Medieninformatik der Hochschule Emden-Leer (Sommer 2021))
# Die zu visualisierenden csv-Dateien müssen im Pfad Assets/StreamingAssets des Unity-Projektes liegen,
# diese py-Datei im Ordner Assets/Scripts/Python.
# Die CSV-Dateien benötigen ein ; als Trennzeichen
# Das Programm erzeugt Diagramme für alle im o.g. Ordner liegenden CSV-Dateien und speichert diese im
# Unterordner Graphs in StreamingAssets
# alternativ kann ein einzelner Dateiname (ohne Dateiendung) beim Aufruf als Kommandozeilenparameter angegeben werden




import pandas as pd
import sys
import os
import altair as alt
alt.renderers.enable('altair_viewer')

# Variablen
global VelocityZeroFrame
VelocityZeroFrame = 0
fileNames = []

# Pfad zu den CSV-Dateien
data_files = '../../StreamingAssets/'

# Unterordner Graphs in StreamingAssets erzeugen (falls noch nicht vorhanden)
if not os.path.exists(data_files + 'Graphs'):
    os.mkdir(data_files + '/Graphs')

# Wenn kein Dateiname als Kommandozeilenargument angegeben wurde,
# alle csv-Dateien im Verzeichnis StreamingAssets plotten
if not len(sys.argv) >= 2:
    for entry in os.listdir(data_files):
        if os.path.isfile(os.path.join(data_files, entry)):
            if entry.endswith(".csv"):
                fileNames.append(entry.split('.')[0])
else:
    fileNames.append(str(sys.argv[1]))

# Array mit Dateinamen (bei Kommandozeilenargument nur ein Eintrag) durchgehen und plotten
for fileName in fileNames:
    # csv-Datei einlesen - deutsches Komma gegen Punkt ersetzen
    file = pd.read_csv(f"{data_files}{fileName}.csv", index_col=False, sep=';', thousands='.', decimal=',')
    # Prüfen, ob Collision vom Unity-Programm erkannt wurde (bool collision = True) und falls ja, ersten Frame merken
    for index, row in file.iterrows():
        if row['VelocityZero']:
            VelocityZeroFrame = row['Frame']
            break
        else:
            continue

    # zu plottende Spalten der Datei auswählen
    # <Name des Werts>:<Spaltenüberschrift aus csv-Datei>
    df = pd.DataFrame({
        'Frames': file['Frame'],
        'Active Velocity Distance': file['ActiveVelocityDistance'],
        'Active Acceleration X': file['ActiveAccelerationX'],
        'Active Acceleration Y': file['ActiveAccelerationY'],
        'Active Acceleration Z': file['ActiveAccelerationZ'],
    })

    # Farbwerte für Linien angeglichen an Unity-Koordinatenfarben
    scale = alt.Scale(
        domain=['Active Velocity Distance', 'Active Acceleration X', 'Active Acceleration Y', 'Active Acceleration Z'],
        range=['yellow', 'red', 'lime', 'blue'])

    # Null-Linie hervorheben zur besseren Übersichtlichkeit
    vertline = alt.Chart().mark_rule().encode(
        x='a:Q',
    )

    # Horizontale Linie bei Frame der vom System erkannten Berührung
    line = alt.Chart(pd.DataFrame({'y': [0]})).mark_rule().encode(y='y')

    # Diagrammlinien zeichnen
    chart_data = alt.Chart(df, width=1024, height=768).mark_line().transform_fold(
        fold=['Active Velocity Distance', 'Active Acceleration X', 'Active Acceleration Y', 'Active Acceleration Z'],
        as_=['variable', 'value']
    ).encode(
        x=alt.X('Frames', axis=alt.Axis(title="Frames", titleFontSize=15, labelFontSize=12, tickMinStep=1)),
        y=alt.Y('value:Q', axis=alt.Axis(title="", labelFontSize=12)),
        color=alt.Color('variable:N', scale=scale,
                        legend=alt.Legend(labelFontSize=16, symbolSize=300, labelLimit=0, title=""))
    ).interactive()

    # Daten zusammenführen
    chart = alt.layer(chart_data, vertline, line, data=df).transform_calculate(a=str(VelocityZeroFrame))

    # Diagramm speichern
    chart.save(f"{data_files}/Graphs/{fileName}.html")
