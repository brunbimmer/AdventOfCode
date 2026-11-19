# AdventOfCode
This project and repository is my main project for solving the daily Advent of Code puzzles that come up every December.

"Advent of Code is an Advent calendar of small programming puzzles for a variety of skill sets and skill levels that can 
 be solved in any programming language you like."

https://adventofcode.com/

All solutions are completed in C# and many of the supporting libraries that assist with reading/parsing inputs and apply
common algorithms to puzzles were build from the ground up with some pointers from other coders that have participated
in this daily puzzle.

Developer Tools required: Visual Studio 2022 w/ .NET Core 6.0


The AdventOfCode project/folder is purposefully missing a key file that is required for the solution. To allow you to 
pull input from the AdventOfCode website and generate your own output based on your custom input files, you must create 
your very own "appSettings.json" and use your own session cookie that is saved into a cookie once you log in. This is 
what you need to do:

1) Create a file called appSettings.json and save it to .\AocSolutions\
2) Copy the following contents into that file:
```
  {
     "session": "<session_id>",
     "advent_of_code_url": "https://adventofcode.com/{0}/day/{1}/input"
  }
```  
3) Visit https://adventofcode.com and sign in with your own account, or a github/FB/google account. 
4) Once logged in, visit any event/day problem set, so as an example: https://adventofcode.com/2021/day/1/input
5) The next steps may differ based on your browser, but right click on the page and select "Inspect"
6) Go to the network tab, and reload the page.
7) Go to the section which will display your "Request Headers" and look for the "cookie" header. Within that header,
   you should see a "session" variable. Copy this variable and replace the "<session_id" token above.


## Running puzzles

Run these commands from the `AocSolutions` directory after configuring `appSettings.json`.

```sh
# Run one puzzle
dotnet run -- 2020 20

# Run every implemented day in 2020, in day order
dotnet run -- 2020 all

# Select a year using a named option
dotnet run -- --year 2020

# Run every implemented puzzle, in year and day order
dotnet run -- --all

# Include solver diagnostics and alternate calculations
dotnet run -- 2020 all --verbose

# Save the table for comparison with your recorded Advent of Code answers
dotnet run --no-build -- 2020 all > results-2020.txt
```

Batch runs print one row per part with the solver's description, computed answer,
expected answer, and reconciliation result. Long descriptions wrap within a 36-character
column. Total puzzle runtime and execution status appear on the first part's row.
Multiline answers and
answers longer than 32 characters appear in labelled blocks below the table, with
references in the answer columns.
Interactive runs show a temporary progress line between rows. Redirected output contains
the table and its associated details.

An exception marks the row as `Error`, preserves answers already reported, and includes
error details below the table. Execution continues with the next day. A batch with
exceptions, mismatched answers, or missing computed answers for recorded expectations
exits with code 1. `(unreported)` identifies a part that supplied no answer,
including parts that a puzzle does not have. `(empty)` identifies a reported empty string.
Solvers that return without reporting either answer have status `No answers`.
`Completed` means the solver returned successfully and reported at least one answer.
Record the answers from Advent of Code in `answers.json` to check correctness.

Use `--verbose` with either a single puzzle or a batch to show diagnostics. For puzzles
with multiple implementations, the table uses the first calculation and verbose output
includes alternate calculations. For 2021 day 7, the average-position calculation supplies
part 2 and the center-of-mass calculation appears in verbose output.

Each puzzle uses the existing input-loading behavior, including `daily.txt`, cached
inputs, downloads for missing inputs, and any puzzle-specific input overrides. Runtime
depends on the individual solvers, and an unfinished or long-running solver can delay
the remaining days.

Solvers report results through the optional `IPuzzleOutput` parameter to `GetSolution`.
Existing calls such as `GetSolution("daily.txt", true)` use console output. Report each
answer once, immediately after computing it, to retain completed parts if a later part
fails.

```csharp
output ??= new ConsolePuzzleOutput(trackTime);
output.WritePart(1, answer, trackTime ? _SW.Elapsed : null, "Answer description");
output.WriteDiagnostic("Optional diagnostic information");
```

## Recording verified answers

`AocSolutions/answers.json` contains blank entries for every implemented puzzle. Review
your answers on Advent of Code and replace each `null` with the verified answer in
quotation marks. The accompanying `answers.schema.json` provides editor validation
through the file's `$schema` property.

```json
{
  "$schema": "./answers.schema.json",
  "puzzles": [
    {
      "year": 2020,
      "day": 1,
      "part1": "<answer copied from Advent of Code>",
      "part2": null
    }
  ]
}
```

Answers are strings, including numeric answers, to preserve large numbers and leading
zeroes. Use `null` or omit a part while its expected answer is unknown. An empty string
is a recorded answer. Each year/day combination can appear once.

`--all`, `--year <year>`, and `<year> all` load the catalog and show one row for each
part, with `Description`, `Computed`, `Expected`, and `Check` columns. Each part has its
own reconciliation result, such as `Match` or `Mismatch`. Missing descriptions display
as `-`.

| Result | Meaning |
| --- | --- |
| Match | The computed answer matches the recorded answer. |
| Mismatch | Both answers exist and differ. |
| Missing | An expected answer exists and the solver supplied no answer. |
| Unknown | An expected answer has yet to be recorded. |

Comparison is case-sensitive and preserves spaces and leading zeroes. Windows and Unix
line endings compare equally, and final newline characters are ignored. Use `\n` in JSON
for multiline answers. Both computed and expected grids appear in labelled blocks below
the table. The summary counts reconciliation results for individual parts.

The runner looks for `answers.json` in the working directory, then
`AocSolutions/answers.json` relative to that directory, then beside the executable.
The catalog and schema are copied to the build output. An absent catalog produces
`Unknown` results. Invalid JSON, invalid puzzle identifiers, and duplicate year/day
entries produce an error before puzzle execution. Unknown expected answers allow a
successful exit when execution completes successfully.
