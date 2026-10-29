# NEVOLAplus: rzeczywista sesja operatora intranetu

Branch: `wersja-inzynierska-2026`. Stan bazowy: `master` / `bd3f7be681dc6d8dfe87a8376278bdd4b595ae6f`. Data wykonania: 29.10.2026.

To autorski wariant rozwojowy konkretnego fragmentu projektu. Branch zachowuje pełne drzewo kodu z wersji bazowej i zawiera opisane poniżej zmiany. [Wcześniejszy audyt](https://github.com/MichalGarnuch/NEVOLAplus/tree/audyt-edukacyjny-2026) jest na osobnym branchu. Nie wykonywano merge ani wdrożenia.

## Zmiana modelu dostępu

Samo przejście do widoku lub sprawdzenie stałego hasła w kontrolerze nie daje bezpiecznej sesji. W intranecie dodałem cookie authentication i fallback wymagający uwierzytelnienia. `AccountController` ma publiczne GET/POST logowania, weryfikację antiforgery, podpisanie tożsamości i wylogowanie wyłącznie POST z antiforgery.

`Security/AdminAccess.cs` jest jawnym trybem jednego operatora. Nazwa i hash hasła są wymagane w `Auth:Username` i `Auth:PasswordHash`; nie ma domyślnych danych dostępowych. Hash musi mieć format ASP.NET Identity, wytworzony przez `PasswordHasher`. Do konfiguracji środowiska służą `Auth__Username` i `Auth__PasswordHash`. Nie wpisuj jawnego hasła zamiast hasha.

## Cykl sesji

Po poprawnej weryfikacji cookie zawiera tożsamość operatora, rolę `Administrator` i wersję danych dostępowych. Wersja jest skrótem nazwy i hasha. Po zmianie konfiguracji i restarcie instancji walidacja odrzuca starsze cookies; nie wystarczy zmienić hasła tylko dla nowych logowań.

Cookie jest HttpOnly, SameSite Strict, ma 30-minutowy czas z odnowieniem i wymaga Secure poza Development. Zmiana danych formularza nie nadaje roli. POST logowania ma limit 5 prób na minutę dla adresu połączenia. Nie ufam bezwarunkowo nagłówkowi X-Forwarded-For; przy proxy trzeba skonfigurować zaufane przekazywanie, inaczej wszyscy użytkownicy mogą dzielić jeden limit.

## Wdrożenie i koszty

Skonfiguruj sekret przed startem. Hash można uzyskać krótkim programem korzystającym z `new PasswordHasher<object>().HashPassword(user, password)` na docelowym .NET; przekaż hasło wejściem chronionym, bez zapisywania go w historii poleceń. Sekrety oraz klucze Data Protection wymagają właściwego magazynu. Przy wielu instancjach potrzebny jest wspólny, chroniony key ring Data Protection; bez niego cookies mogą nie działać pomiędzy instancjami.

To rozwiązanie dla jednego operatora, nie pełny katalog pracowników. Docelowo zastosowałbym Identity/OIDC, indywidualne konta, MFA i uprawnienia do operacji kadrowych. Fallback nie zastępuje izolacji poszczególnych pracowników ani audytu dostępu.

## Testy i dalszy plan

Test rzeczywistej klasy `AdminAccess` sprawdza poprawne hasło, niewłaściwą nazwę, niewłaściwe hasło, zmianę wersji po rotacji i odmowę braku konfiguracji. Kontroler też skompilowano względem referencji ASP.NET 8. Następne potrzebne testy to pełny HTTP cookie/antiforgery, odrzucenie sesji po rotacji i działanie limitu za docelowym proxy. Zmiana dotyczy intranetu; portal nie został przebudowany na kompletny system kont.

## Weryfikacja wykonana w tej sesji

| Sprawdzenie | Wynik i znaczenie |
|---|---|
| engineering-tests: AdminAccess przez Roslyn/.NET 8 | PASS: 5 sprawdzeń danych dostępowych |
| Kompilacja AccountController z AdminAccess | PASS: referencje ASP.NET 8.0.33 |
| Pełna solucja / SQL Server / test HTTP cookies | NIEWYKONANY |

### Granice weryfikacji

MSBuild nie inicjalizuje się poprawnie w tym środowisku; nie uruchomiono całej aplikacji EF/Razor. Nie zweryfikowano widoków w przeglądarce, rzeczywistego proxy, Data Protection pomiędzy replikami ani integracji z bazą.

## Mapa zmian

- [`NEVOLAplus.Intranet/Security/AdminAccess.cs`](NEVOLAplus.Intranet/Security/AdminAccess.cs)
- [`engineering-tests/AdminAccess.Tests.csproj`](engineering-tests/AdminAccess.Tests.csproj)
- [`engineering-tests/Program.cs`](engineering-tests/Program.cs)
- [`NEVOLAplus.Intranet/Controllers/AccountController.cs`](NEVOLAplus.Intranet/Controllers/AccountController.cs)
- [`NEVOLAplus.Intranet/Program.cs`](NEVOLAplus.Intranet/Program.cs)
- [`NEVOLAplus.Intranet/Views/Account/Login.cshtml`](NEVOLAplus.Intranet/Views/Account/Login.cshtml)
- [`NEVOLAplus.Intranet/Views/Shared/_Layout.cshtml`](NEVOLAplus.Intranet/Views/Shared/_Layout.cshtml)
