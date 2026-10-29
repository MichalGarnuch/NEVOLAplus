# NEVOLAplus — przewodnik i audyt edukacyjny

**Stan przeglądu:** 29 października 2026 r.  
**Gałąź bazowa:** `master`  
**Analizowany commit:** `bd3f7be681dc6d8dfe87a8376278bdd4b595ae6f`  
**Mapa wszystkich plików śledzonych przez Git:** [1](AUDYT_MAPA_01.md)

> To jest przegląd reprezentatywnych części projektu z inwentaryzacją wszystkich śledzonych plików, a nie pełna, ręczna weryfikacja każdego wiersza ani certyfikat bezpieczeństwa. Mapa jest automatyczna; wymienienie pliku nie oznacza, że został przetestowany lub dokładnie przeczytany. Wyniki odnoszą się do powyższego commita. Inne gałęzie mogą zawierać inne wersje.

## Co robi projekt

Portal i intranet ASP.NET Core z modułami CMS, pracowników i zasobów.

## Jak czytać kod i jak przepływają dane

[`NEVOLAplus.Data/`](NEVOLAplus.Data) zawiera modele i kontekst EF, [`NEVOLAplus.Portal/`](NEVOLAplus.Portal) część publiczną, a [`NEVOLAplus.Intranet/`](NEVOLAplus.Intranet) kontrolery oraz widoki zarządzające.

Podczas czytania zacznij od punktu wejścia, przejdź do funkcji odpowiadającej za żądanie lub działanie użytkownika, a potem do operacji na danych i wyniku. Mapa plików podaje ścieżki i automatycznie rozpoznane nazwy wybranych deklaracji.

## Co jest wartościowe w obecnym rozwiązaniu

Rozdzielenie portalu, intranetu i danych pokazuje klasyczny podział aplikacji MVC. Wybrane formularze mają ochronę anty-CSRF.

## Ustalenia, luki i proponowany rozwój

- **Wysoki priorytet — logowanie demonstracyjne:** [`NEVOLAplus.Intranet/Controllers/AccountController.cs`](NEVOLAplus.Intranet/Controllers/AccountController.cs#L14) po wysłaniu loginu i hasła jedynie przekierowuje na stronę główną. `Program.cs` nie konfiguruje uwierzytelniania, a przejrzany kontroler pracowników nie ma ograniczenia dostępu. Nie używaj tego jako ochrony prawdziwych danych.

- **Do rozwoju:** wdrożyć właściwe uwierzytelnianie, role i testy dostępu do danych pracowników; przechowywać połączenia do bazy poza kodem i regularnie wykonywać kopie.

- **Do sprawdzenia:** spójność kodowania polskich znaków w części plików C# i widoków.

## Sprawdzenia i ich granice

Nie uruchomiono .NET, widoków ani SQL Server z powodu braku środowiska. Nie stwierdzono, czy istnieje działające publiczne wdrożenie.

W kopii roboczej dostępne pliki Pythona sprawdzono dodatkowo parserem składni; sprawdzono też składnię odczytanych plików `package.json` i `composer.json`. Nie zastępuje to uruchomienia programu. Kod źródłowy tej gałęzi jest taki sam jak w commicie bazowym; dodane są wyłącznie dokumenty audytu i mapy.

## Jak zaplanować kolejne prace

Najpierw zajmij się punktami oznaczonymi jako krytyczne lub wysokie, następnie ustal, które uwagi wymagają testu na pełnym środowisku. Dla każdej zmiany przygotuj osobną gałąź, konkretny przypadek odtwarzający problem i test rezultatu. Nie traktuj propozycji jako potwierdzenia, że poprawka została już wdrożona.
