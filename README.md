# Conrad Adventskalender für Raspberry Pi (2016) mit Windows IoT

Ich habe einen Conrad Adventskalender für Raspberry Pi geschenkt bekommen. 😍   
In der Anleitung zum Adventskalender steht im einleitenden Text

    Außerdem ist das Windows-Betriebssystem denkbar ungeeiget, mit Elektronik zu kommunizieren.

Na das wollen wir doch einmal sehen! 😄 Ich werde versuchen die Schaltungen von jedem Tag als Windows IoT Projekt hier bereit zu stellen. Das wird ein Spaß! 😏

## Tag 1
* Windows 10 IoT installiert, wie hier beschrieben: https://developer.microsoft.com/de-de/windows/iot/Docs/GetStarted/rpi2/sdcard/insider/getstartedstep1
* 1. Schaltung aufgebaut
* Dieses Repository aufgesetzt :smirk:

## Tag 2
* Ich habe die IoT-Vorlagen von hier heruntergeladen und installiert: https://marketplace.visualstudio.com/items?itemName=MicrosoftIoT.WindowsIoTCoreProjectTemplates
* Danach habe ich ein neues C#-Projekt mit dem Namen "Tag2" erstellt, wie hier beschrieben: https://developer.microsoft.com/de-de/windows/iot/docs/backgroundapplications
* Als Nächstes habe ich einen Verweis auf die "Windows IoT Extensions for the UWP" hizugefügt
    * "Projektmappen-Explorer" > Rechtsklick auf "Verweise" > "Verweis hinzufügen..." > "Universal Windows" > "Erweiterungen" > "Windows IoT Extensions for the UWP" anhanken
* Zum Schluß habe ich das Microsoft Blink-beispiel verwendet und angepasst, und damit die Aufgabe von Tag 2 mit Windows IoT gelöst :metal: https://developer.microsoft.com/en-us/windows/iot/Samples/HelloBlinkyBackground

## Tag 3
* Neues Projekt "Tag3" erstellt :smirk:
* Heute gab es eine Farbe mehr im Wechsel. Funktioniert auch! :smile:

## Tag 4
* Heute Farbenspiel per Zufall, funktioniert ebenfalls

## Tag 5
* Pullup-Wiederstand an GPIO Pin ausgeschalten, wie hier beschrieben: https://developer.microsoft.com/de-de/windows/iot/docs/PinMappingsRPi#gpio-sample
* Sensorkontakt funktioniert wie im Beispiel

## Tag 6
* Heute Sound-Erzeugung. Gestaltet sich leider etwas komplizierter. Morgen gibt es mehr!