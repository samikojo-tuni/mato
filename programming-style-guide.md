# Game Programming Style Guide for C# language

## General Principles

1. **Consistency**: Maintain a consistent style throughout the codebase to improve readability and maintainability.
2. **Clarity**: Write clear and understandable code. Avoid complex and convoluted constructs.
3. **Simplicity**: Keep the code as simple as possible. Avoid unnecessary complexity.

## Naming Conventions

1. **PascalCase** for class names, methods, and properties.

```csharp
public class PlayerCharacter
{
    public void MoveForward()
    {
    }

    public int Health { get; set; }
}
```

2. **camelCase** for local variables and method parameters.

```csharp
public void UpdateScore(int newScore)
{
    int score = newScore;
}
```

3. **ALL_CAPS** for constants.

```csharp
const int MAX_HEALTH = 100;
```

4. **_camelCase** for member variables (fields).

```csharp
private int _playerHealth;
```

## Layout and Formatting

1. **Indentation**: Use tabs for indentation. Do NOT use spaces.
2. **Braces**: Place opening braces on a new line.

```csharp
if (isAlive)
{
    // code
}
else
{
    // code
}
```

3. **Spacing**: Use a single space before and after operators.

```csharp
int sum = a + b;
```

## Commenting

1. **Summary Comments**: Use XML comments for public members.

```csharp
/// <summary>
/// Moves the player forward.
/// </summary>
public void MoveForward()
{
}
```

2. **Inline Comments**: Use inline comments sparingly and only when necessary to explain complex logic.

```csharp
// Check if the player is alive
if (isAlive)
{
    // code
}
```

## Code Structure

1. **Methods**: Keep methods short and focused. Each method should perform a single task.
2. **Classes**: Follow the Single Responsibility Principle. Each class should have a single responsibility.
3. **Regions**: Use regions to organize code into logical sections.

```csharp
#region Player Movement
// code
#endregion
```

## Best Practices

1. **Avoid Magic Numbers**: Use constants or enums instead of hard-coded numbers.

```csharp
const int MAX_PLAYERS = 4;
```

2. **Error Handling**: Use try-catch blocks to handle exceptions gracefully.

```csharp
try
{
    // code
}
catch (Exception ex)
{
    // handle exception
}
```

3. **Access modifiers**: Use access modifiers when declaring fields, properties and methods.

```csharp
private int _health = 0;

public int Health
{
    get;
    private set;
}

public void IncreaseHealth(int amount)
{
    // code
}
```

4. **Field initialization**: Initialize fields with reasonable default values.

```csharp
private int _health = 100;
private float _speed = 5.0f;
private IMover _mover = null;
```

5. **Code Reviews**: Regularly review code with peers to ensure quality and consistency.
