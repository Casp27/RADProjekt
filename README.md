# Implementeringsprojekt — RAD

## Krav
- .NET 8.0 SDK
- ScottPlot 5.1.58

## Sådan kører du projektet

Gå til projektmappen og kør

```bash
dotnet run -- [opgave]
```

### Opgaver

| Kommando | Beskrivelse |
|---|---|
| `dotnet run -- 1` | Opgave 1 |
| `dotnet run -- 2` | Opgave 2 og 3 |
| `dotnet run -- 3` | Opgave 4 og 5 |
| `dotnet run -- 6` | Opgave 6 |
| `dotnet run -- 7` | Opgave 7 |
| `dotnet run -- 8` | Opgave 8 |


## Eksempel

```bash
dotnet run -- 2
```

Giver output som:
- Manuel test af hashtabellen
- Tabel med køretider for MS og MMP ved stigende værdier af l
- Stopper automatisk når køretiden overstiger 5000ms

## Projektstruktur

| Fil | Indhold |
|---|---|
| `Del_1_Opgave_1.cs` | Multiply-Shift og Multiply-Mod-Prime hashfunktioner |
| `Del_1_Opgave_2_Og_3.cs` | Hashtabel med chaining og beregning af kvadratsummer |
| `Del_2_Opgave_4.cs` | 4-universel hashfunktion g(x) og Count-Sketch hashfunktioner h og s |
| `Del_2_Opgave_6.cs` | Count-Sketch implementering |
| `Del_2_Opgave_7.cs` | Eksperimenter med Count-Sketch |
| `Del_2_Opgave_8.cs` | Eksperimenter med betydning af m |
| `Program.cs` | Indgangspunkt — styrer hvilken opgave der køres |
