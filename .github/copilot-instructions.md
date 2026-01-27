# AI Rules for 10xCards

{{project-description}}

## Tech Stack

- C# 14
- .NET 10.0.2
- BlazorWebAssembly 10.0.2
- Bootstrap v5.3.3

## Project Structure

When introducing changes to the project, always follow the directory structure below:

- `./Pages` - Blazor pages
- `./Layout` - Blazor layouts

When modifying the directory structure, always update this section.

## Coding practices

### Guidelines for clean code

- Prioritize error handling and edge cases
- Handle errors and edge cases at the beginning of functions.
- Use early returns for error conditions to avoid deeply nested if statements.
- Place the happy path last in the function for improved readability.
- Avoid unnecessary else statements; use if-return pattern instead.
- Use guard clauses to handle preconditions and invalid states early.
- Implement proper error logging and user-friendly error messages.
- Consider using custom error types or error factories for consistent error handling.