# NEVOLAplus

NEVOLAplus to przykładowa aplikacja webowa zbudowana w oparciu o **ASP.NET Core 8**. Projekt składa się z trzech modułów:

- **NEVOLAplus.Data** – biblioteka z modelami i kontekstem Entity Framework Core
- **NEVOLAplus.Portal** – publiczny portal z prostym systemem CMS
- **NEVOLAplus.Intranet** – panel administracyjny do zarządzania danymi

Poniżej znajdują się wskazówki jak uruchomić aplikację lokalnie.

## Architektura

### Warstwa danych

Kontekst `NevolaIntranetContext` definiuje tabele odpowiadające za CMS, moduł HR czy inwentarz. Fragment pliku:

```csharp
// CMS
public DbSet<Page> Pages { get; set; } = null!;
public DbSet<News> News { get; set; } = null!;
public DbSet<TextSnippet> TextSnippets { get; set; } = null!;

// HR
public DbSet<Employee> Employees { get; set; } = null!;
public DbSet<Position> Positions { get; set; } = null!;

// Inventory
public DbSet<Asset> Assets { get; set; } = null!;
public DbSet<AssetType> AssetTypes { get; set; } = null!;

// Reservation
public DbSet<Reservation> Reservations { get; set; } = null!;

// Licensing
public DbSet<SoftwareLicense> SoftwareLicenses { get; set; } = null!;
```

### Portal (część publiczna)

Portal wyświetla treści z bazy danych. Do pobierania tekstów używany jest `TextSnippetService`:

```csharp
public async Task<string?> GetContentByKeyAsync(string key)
{
    return await _context.TextSnippets
                         .Where(s => s.Key == key)
                         .Select(s => s.Content)
                         .FirstOrDefaultAsync();
}
```

Serwis jest wstrzykiwany w widokach (`@inject ITextSnippetService`) i umożliwia tłumaczenie etykiet menu czy nagłówków stron.

### Intranet (panel administracyjny)

Intranet zawiera kontrolery CRUD do zarządzania danymi oraz narzędzie do edycji stylów portalu. Fragment `StyleController` zapisywujący własny arkusz CSS:

```csharp
public IActionResult Edit(string cssContent)
{
    var path = Path.Combine(_env.ContentRootPath, "..", "NEVOLAplus.Portal", "wwwroot", "css", "custom.css");
    System.IO.File.WriteAllText(path, cssContent ?? string.Empty);
    return RedirectToAction(nameof(Edit));
}
```

## Wymagania

- Zainstalowany [SDK .NET 8](https://dotnet.microsoft.com/en-us/download)
- Dostęp do lokalnej instancji MS SQL Server lub `localdb` (domyślne ustawienia w `appsettings.json`)
- Opcjonalnie [Docker](https://www.docker.com/) do uruchamiania aplikacji w kontenerach

## Szybki start

1. Sklonuj repozytorium i przejdź do katalogu projektu:
   ```bash
   git clone <adres-repo>
   cd NEVOLAplus
   ```
2. Przywróć zależności i utwórz bazę danych:
   ```bash
   dotnet restore
   dotnet ef database update --project NEVOLAplus.Intranet --startup-project NEVOLAplus.Intranet
   ```
3. Uruchom wybraną aplikację:
   ```bash
   dotnet run --project NEVOLAplus.Portal
   # lub
   dotnet run --project NEVOLAplus.Intranet
   ```
   Domyślne adresy to `http://localhost:5195` dla portalu oraz `http://localhost:5236` dla intranetu.

## Konfiguracja

Podstawowe opcje znajdują się w plikach `appsettings.json`:

- `ConnectionStrings:NevolaIntranetContext` – ścieżka do bazy danych. Można ją nadpisać zmienną środowiskową `ConnectionStrings__NevolaIntranetContext`.
- `CustomCssPath` – opcjonalna ścieżka do własnego pliku CSS używanego przez portal.

Zmiany w konfiguracji można wprowadzać przed uruchomieniem aplikacji lub poprzez zmienne środowiskowe.

## Uruchamianie w Dockerze

Każdy moduł posiada własny `Dockerfile`. Przykładowe polecenia dla portalu:

```bash
docker build -t nevola-portal -f NEVOLAplus.Portal/Dockerfile .
docker run --rm -p 8080:8080 -p 8081:8081 nevola-portal
```

Analogicznie można zbudować obraz `NEVOLAplus.Intranet`. Zmienna `APP_UID` w `Dockerfile` pozwala określić identyfikator użytkownika w kontenerze.

## Zrzuty ekranu

### Logowanie

![Zrzut ekranu 2025-07-19 180042](https://github.com/user-attachments/assets/1bcd7a50-07b3-44a1-a297-f6b60279a4ef)

### Strony główne

![Zrzut ekranu 2025-07-19 175915](https://github.com/user-attachments/assets/7182830f-9d7a-4106-b510-074e3283a892)

![Zrzut ekranu 2025-07-19 180528](https://github.com/user-attachments/assets/7ef6703a-0b96-426e-ab8e-736738cde9eb)

### Kilka Pagesów

![Zrzut ekranu 2025-07-19 180459](https://github.com/user-attachments/assets/44e49c1a-b494-4e0b-a43e-fbcd2bde44a7)

![Zrzut ekranu 2025-07-19 180426](https://github.com/user-attachments/assets/6cbb10ef-5ac8-4048-b544-92d589145a9d)

![Zrzut ekranu 2025-07-19 180355](https://github.com/user-attachments/assets/4242b361-02be-46f7-b505-77358d7ceddb)

![Zrzut ekranu 2025-07-19 180309](https://github.com/user-attachments/assets/1178fa76-62ea-40c8-ac72-850df5254162)

![Zrzut ekranu 2025-07-19 180134](https://github.com/user-attachments/assets/2c943e5d-cae3-4270-affb-f6cb7df775c5)

![Zrzut ekranu 2025-07-19 180726](https://github.com/user-attachments/assets/98c16a83-5650-4596-8812-d08ebabfcb6f)

## Struktura projektu

```
NEVOLAplus.sln
├── NEVOLAplus.Data
│   └── Models, Migrations, Context
├── NEVOLAplus.Portal
│   ├── Controllers, Views
│   └── wwwroot (statyczne zasoby)
└── NEVOLAplus.Intranet
    ├── Controllers, Views
    └── wwwroot
```
