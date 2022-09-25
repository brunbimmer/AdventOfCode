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


More info to follow.
