# Advent of Code 2025 - Day 10: Factory

## Problem Overview

You need to configure factory machines by pressing buttons. Each machine has:
- **Indicator lights** (Part 1) - Binary on/off state
- **Joltage counters** (Part 2) - Numeric values

Each button, when pressed, toggles/increments specific lights or counters.

**Goal:** Find the minimum total button presses needed to reach the target configuration.

---

## Part 1: Indicator Light Configuration

### Input Format
```
[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
```

- `[.##.]` - Target state: light 0 off, lights 1-2 on, light 3 off
- `(3)`, `(1,3)`, etc. - Button definitions (which lights each button toggles)
- `{3,5,4,7}` - Joltage requirements (ignored in Part 1)

### Algorithm: BFS (Breadth-First Search)

Since we want the **minimum** presses, BFS guarantees we find it first:

1. Start with all lights **off** (initial state)
2. Try pressing each button (one at a time)
3. Track visited states (state = which lights are on)
4. Stop when we reach the target state
5. Return the number of presses taken

**Key insight:** We only care about parity (on/off), not how many times pressed. If you press a button twice, it's the same as not pressing it at all. So we use modulo 2 for state tracking.

---

## Part 2: Joltage Counter Configuration

### Input Format (Same as Part 1)
```
[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
```

- `[.##.]` - Ignored in Part 2
- `(3)`, `(1,3)`, etc. - Button definitions (which counters each button increments)
- `{3,5,4,7}` - **Target joltage values**: counter 0 must reach 3, counter 1 must reach 5, etc.

### Algorithm: Gaussian Elimination with Free Variable Enumeration

This is a system of linear equations problem:

**Mathematical formulation:**
- Variables: $x_0, x_1, ..., x_{n-1}$ = number of times each button is pressed
- Equations: One equation per counter, stating the sum of contributions equals target

For example:
```
Counter 0: button_0 affects it, button_3 affects it, button_4 affects it, button_5 affects it
         x_0 + x_3 + x_4 + x_5 = 3

Counter 1: button_1, button_3, button_5 affect it
         x_1 + x_3 + x_5 = 5
```

#### Steps:

1. **Build the coefficient matrix**
   ```
   [1 0 0 1 1 1 | 3]   (Counter 0 equation)
   [0 1 0 1 0 1 | 5]   (Counter 1 equation)
   [0 0 1 1 0 0 | 4]   (Counter 2 equation)
   [0 0 0 1 0 0 | 7]   (Counter 3 equation)
   ```
   - Rows = counters
   - Columns = buttons (+ augmented column for targets)
   - Entry [i,j] = 1 if button j affects counter i, else 0

2. **Gaussian Elimination**
   - Reduce matrix to row echelon form
   - Identify **pivot columns** (dependent variables)
   - Identify **free columns** (free variables)
   
   Example: If we have 4 equations and 6 buttons, we might have 4 pivot variables and 2 free variables.

3. **Enumerate Free Variables**
   - Try all combinations of free variable values (0 to max joltage requirement)
   - For each combination, use back-substitution to find dependent variable values
   - Verify all variables are non-negative integers
   - Track the solution with **minimum total presses** (sum of all variables)

#### Why This Works:

- **Determined systems** (# equations = # unknowns): Unique solution exists
- **Underdetermined systems** (# unknowns > # equations): Infinite solutions exist (free variables)
  - We enumerate them intelligently by trying all reasonable combinations
- **Overdetermined systems** (# equations > # unknowns): May have no solution or unique solution

### Example: First Test Machine

```
[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
6 buttons, 4 counters
```

**Equations:**
- Counter 0: x_0 + x_3 + x_4 + x_5 = 3
- Counter 1: x_1 + x_3 + x_5 = 5  
- Counter 2: x_2 + x_3 = 4
- Counter 3: x_3 = 7

After Gaussian elimination:
- Pivot variables: some subset (dependent on elimination)
- Free variables: remaining buttons
- Enumerate free variable values
- Result: **10 total presses** is minimum

---

## Code Structure

### Main Classes

- **Machine** - Represents a single machine with:
  - `Target`: Indicator light diagram (string like "[.##.]")
  - `Buttons`: List of button definitions (each is a list of affected indices)
  - `JoltageRequirements`: Target joltage values

### Key Methods

#### `SolvePart1(List<Machine> machines)`
- Sums the minimum presses for each machine
- Uses BFS per machine

#### `FindMinimumPressesLight(Machine machine)`
- BFS to find minimum button presses for indicator lights
- Returns count of presses when target state is reached

#### `SolvePart2(List<Machine> machines)`
- Sums the minimum presses for joltage configuration
- Uses Gaussian elimination per machine

#### `FindMinimumPressesJoltage(Machine machine)`
- Builds coefficient matrix
- Calls Gaussian elimination
- Enumerates free variables if system is underdetermined
- Returns minimum total presses

#### `GaussianEliminationReal(double[,] matrix, int rows, int cols)`
- Reduces matrix to row echelon form using floating-point arithmetic
- Returns reduced matrix and list of pivot column indices
- Handles:
  - Row swaps for numerical stability
  - Pivoting with partial selection
  - Elimination in all rows (not just below pivot)

#### `EnumerateFreeVariables(...)`
- For systems with free variables, tries all combinations
- Uses recursive enumeration with pruning
- Computes dependent variables via back-substitution
- Validates solutions (all non-negative integers)

---

## Complexity Analysis

### Part 1: BFS
- Time: O(2^n) worst case (n = number of lights), but typically much better with pruning
- Space: O(2^n) for visited states

### Part 2: Gaussian Elimination + Enumeration
- Gaussian elimination: O(n³) where n = number of buttons
- Enumeration: O(m^f) where m = max joltage and f = number of free variables
- Practical performance: Fast for most inputs since f is typically small

---

## Key Insights

1. **Part 1**: Parity matters only - pressing a button twice = not pressing it
2. **Part 2**: This is fundamentally a system of linear equations
3. **Free variables**: When the system is underdetermined, we need to intelligently search the solution space
4. **Back-substitution**: Once free variables are assigned, dependent variables are determined by the reduced matrix
5. **Integer solutions**: We need all variables to be non-negative integers, so we must validate each solution

---

## Test Case Example

All three test machines:
```
[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
Result: 10 presses

[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
Result: 12 presses

[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
Result: 11 presses

Total: 10 + 12 + 11 = 33 ✓
```
