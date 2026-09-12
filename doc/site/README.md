# EasyParsing Lab

An interactive introduction to EasyParsing and parser combinators.

- **Use it** explains installation from the repository's GitHub Packages NuGet feed and includes a copyable C# example.
- **Understand it** visualizes the decimal pipeline, ordered alternatives and backtracking, and the recursive JSON sample. Playback, stepping, the input cursor, parser calls, captures and the remaining context stay synchronized.
- English is the default; French is available. Light/dark mode follows the system until a preference is selected. Language and theme preferences are stored locally in the browser.

## Run locally

From the repository root, with Node.js 22 or later:

```sh
node doc/serve-site.mjs
```

Open [http://127.0.0.1:4173](http://127.0.0.1:4173). Stop the server with Ctrl+C. An optional port can be passed as the first argument, for example `node doc/serve-site.mjs 8080`.

There is no package installation or build step. Any static HTTP server can serve this directory. JavaScript modules require HTTP rather than opening `index.html` through `file://`. All asset paths are relative, so the site also works under a subdirectory. The entry points are `#use-it` and `#understand`.

Parsing runs in the browser. Google Fonts is optional and falls back to local system fonts; source and authentication links point to GitHub. Hosting/account metadata is not needed to serve the site.

## Validation

```sh
node doc/check-site.mjs
```

This checks asset references, JavaScript syntax, representative parser results, cursor rewinding, English/French messages, and saved/system theme behavior. It does not run a browser.

The original adaptation was also compared against the C# implementation on 594 valid, invalid and truncated inputs. Success/failure, position, result type and value, and the presence of exceptions agreed. The C# quick-start example was compiled and returned `123.456`.

## Fidelity and limits

`engine.mjs` is an instrumented JavaScript adaptation of the `develop` implementation reviewed on September 12, 2026. It does not execute the .NET library. `Cast`, `AsString`, and nested `OrElse` wrappers are folded in the educational trace. Keep the adaptation and the explanatory notes in sync when the C# implementation changes.

The sample's behavior is deliberately preserved:

- Parsers may accept a prefix without consuming the entire input.
- Single quotes and case-insensitive booleans are accepted. `null`, negative numbers and exponents are not part of this sample's grammar.
- `SeparatedBy` currently rejects empty and single-item collections.
- Directly nested arrays can raise `NullReferenceException` because the array alternative captures `ItemsParser` during static initialization.
- Conversion exceptions stop parsing instead of trying another alternative.

For presentation-sized inputs the demo limits input to 800 UTF-16 code units, traces to 14,000 events, and recursion depth. Decimal conversion is limited to 28 significant digits and 28 decimal places; .NET rounding/overflow beyond that range is not simulated. Numeric values are displayed without converting them to JavaScript floating-point numbers.

See the [C# JSON parser](../../src/EasyParsing.Samples.Json/JsonParser.cs) and the [repository license](../../LICENSE). The included `EasyParsing-LICENSE.txt` retains the attribution when this site folder is hosted separately.

## Français

Lancer `node doc/serve-site.mjs` depuis la racine du dépôt, puis ouvrir [http://127.0.0.1:4173](http://127.0.0.1:4173). Le sélecteur FR/EN conserve la saisie et l'étape courante. Le site présente l'installation NuGet, les parser combinators et une visualisation pas à pas du parsing. Le moteur est une adaptation JavaScript pédagogique du code C#, dont les limites sont documentées dans l'interface.
