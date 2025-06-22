# NEVOLAplus

NEVOLAplus to przykładowa aplikacja webowa zbudowana w oparciu o **ASP.NET Core 8**. Projekt składa się z dwu oddzielnych modułów:

- **NEVOLAplus.Portal** – publiczny portal z prostym systemem CMS
- **NEVOLAplus.Intranet** – panel administracyjny i zaplecze zarządzające danymi

Poniżej znajdują się wskazówki jak uruchomić aplikację lokalnie.

## Wymagania

- Zainstalowany [SDK .NET 8](https://dotnet.microsoft.com/en-us/download)
- Dostęp do lokalnej instancji MS SQL Server lub `localdb` (domyślne ustawienie w plikach `appsettings.json`)
- Opcjonalnie [Docker](https://www.docker.com/) jeśli chcesz uruchamiać obrazy kontenerów

## Szybki start

1. Sklonuj repozytorium i przejdź do katalogu projektu:
   ```bash
   git clone <adres-repo>
   cd NEVOLAplus
   ```
2. Przywróć zależności i przygotuj bazę danych:
   ```bash
   dotnet restore
   dotnet ef database update --project NEVOLAplus.Intranet --startup-project NEVOLAplus.Intranet
   ```
   Polecenie `dotnet ef database update` utworzy bazę z wykorzystaniem migracji Entity Framework.
3. Uruchom wybraną aplikację:
   ```bash
   dotnet run --project NEVOLAplus.Portal
   # lub
   dotnet run --project NEVOLAplus.Intranet
   ```
   Domyślne adresy to `http://localhost:5195` dla portalu i `http://localhost:5236` dla intranetu.

## Konfiguracja

Ustawienia aplikacji znajdują się w plikach `appsettings.json`. Najważniejsze opcje to:

- `ConnectionStrings:NevolaIntranetContext` – ścieżka połączenia do bazy danych. Można ją nadpisać zmienną środowiskową `ConnectionStrings__NevolaIntranetContext`.
- `CustomCssPath` – opcjonalna ścieżka do własnego pliku CSS wykorzystywanego przez portal.

Zmiany w konfiguracji można wprowadzać przed uruchomieniem aplikacji lub poprzez zmienne środowiskowe.

## Uruchamianie w Dockerze

Każdy moduł posiada własny `Dockerfile`. Przykładowa komenda budująca i uruchamiająca portal:

```bash
docker build -t nevola-portal -f NEVOLAplus.Portal/Dockerfile .
docker run --rm -p 8080:8080 -p 8081:8081 nevola-portal
```

Dla intranetu należy wskazać `NEVOLAplus.Intranet/Dockerfile`. Zmienna `APP_UID` w `Dockerfile` może być użyta do określenia identyfikatora użytkownika w kontenerze.

## Zrzuty ekranu

Kilka przykładowych widoków z aplikacji:

### Logowanie
![Zrzut ekranu 2025-06-21 170354](https://github.com/user-attachments/assets/5e7c739f-b6f5-4f72-b756-f76a77d8bd56)
![Zrzut ekranu 2025-06-21 170430](https://github.com/user-attachments/assets/cd32a675-4f16-493e-942e-7237b7689871)

### Strony główne
![Zrzut ekranu 2025-06-21 170216](https://github.com/user-attachments/assets/48b9706d-e556-48f8-bf2a-548c6fa97e63)
![Zrzut ekranu 2025-06-21 170331](https://github.com/user-attachments/assets/83728e78-b46b-4d6d-805a-4beb2c69cddd)

### Kilka Pagesów
![Zrzut ekranu 2025-06-21 170501](https://github.com/user-attachments/assets/bc2dbd67-f88d-41b6-8702-a2e269df7afd)
![Zrzut ekranu 2025-06-21 170542](https://github.com/user-attachments/assets/9571e884-f1a1-4e10-9a6c-7cf429c29112)
![Zrzut ekranu 2025-06-21 170609](https://github.com/user-attachments/assets/15018982-cf2b-4b35-aeb0-09b48770ba57)
![Zrzut ekranu 2025-06-21 170649](https://github.com/user-attachments/assets/d1cdf738-b4e7-482c-b004-a635a0feb6a8)
![Zrzut ekranu 2025-06-21 170726](https://github.com/user-attachments/assets/18974344-e36d-4ba8-8604-1b51636ae9de)
![Zrzut ekranu 2025-06-21 170748](https://github.com/user-attachments/assets/0b2fbdda-9983-4680-bd96-a83800bf1045)
![Zrzut ekranu 2025-06-21 170821](https://github.com/user-attachments/assets/9504fa83-19c8-493f-833e-46d072c33813)

### Struktura projektu
![Zrzut ekranu 2025-06-21 171108](https://github.com/user-attachments/assets/2256f148-7499-4423-a362-ffd4031258b7)
