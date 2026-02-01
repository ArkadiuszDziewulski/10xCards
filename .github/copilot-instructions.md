# AI Rules for 10xCards

{{project-description}}

## Tech Stack

- C# 14
- .NET 10.0.2
- BlazorWebAssembly 10.0.2
- Bootstrap v5.3.3
- supabase-csharp 0.16.2

## Project Structure

When introducing changes to the project, always follow the directory structure below:

- `./Pages` - Blazor pages
- `./Layout` - Blazor layouts
- `./Components` - Reusable Blazor components
- `./Models` - Data Models
- `./Services` - API clients and services
- `./docs` - Project documentation
- `./ai` - Planing AI file

When modifying the directory structure, always update this section.

## Coding practices

### Guidelines for clean code

- Prioritize error handling and edge cases.
- Handle errors and edge cases at the beginning of functions.
- Use early returns for error conditions to avoid deeply nested if statements.
- Place the happy path last in the function for improved readability.
- Avoid unnecessary else statements; use if-return pattern instead.
- Use guard clauses to handle preconditions and invalid states early.
- Implement proper error logging and user-friendly error messages.
- Consider using custom error types or error factories for consistent error handling.

### Guidelines for SUPPORT_LEVEL

#### SUPPORT_BEGINNER

- When running in agent mode, execute up to 3 actions at a time and ask for approval or course correction afterwards.
- Write code with clear variable names and include explanatory comments for non-obvious logic. Avoid shorthand syntax and complex patterns.
- Provide full implementations rather than partial snippets. Include import statements, required dependencies, and initialization code.
- Add defensive coding patterns and clear error handling. Include validation for user inputs and explicit type checking.
- Suggest simpler solutions first, then offer more optimized versions with explanations of the trade-offs.
- Briefly explain why certain approaches are used and link to relevant documentation or learning resources.
- When suggesting fixes for errors, explain the root cause and how the solution addresses it to build understanding. Ask for confirmation before proceeding.
- Offer introducing basic test cases that demonstrate how the code works and common edge cases to consider.

### Guidelines for DOCUMENTATION

#### DOC_UPDATES

- Update relevant documentation in /docs when modifying features.
- Keep README.md in sync with new capabilities.
- Maintain changelog entries in CHANGELOG.md.

## Preferencje jêzykowe

- Odpowiedzi powinny byæ udzielane w jêzyku polskim.
