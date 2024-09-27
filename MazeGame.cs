public class MazeGame
{
    private static char[,] maze;
    private readonly static int width = 21;
    private readonly static int height = 21;
    private readonly static Random random = new();
    private static int playerX = 1, playerY = 1;  // Позиция игрока
    private static int finishX, finishY;  // Позиция финиша
    private static bool pathVisible = false; // Отображение пути

    private static void Main()
    {
        Console.CursorVisible = false;
        GenerateMaze();
        DrawMazeAndDisplayHelp();

        // Игровой цикл
        while (true)
        {
            var key = Console.ReadKey(true).Key;

            // Управление игроком и вызов функций
            switch (key)
            {
                case ConsoleKey.W:
                    MovePlayer(-1, 0);  // Вверх
                    break;
                case ConsoleKey.S:
                    MovePlayer(1, 0);   // Вниз
                    break;
                case ConsoleKey.A:
                    MovePlayer(0, -1);  // Влево
                    break;
                case ConsoleKey.D:
                    MovePlayer(0, 1);   // Вправо
                    break;
                case ConsoleKey.F:
                    ShowPathToFinish();
                    break;
                case ConsoleKey.Escape:
                    Console.SetCursorPosition(0, height);
                    Console.WriteLine("Игра завершена.");
                    return;
                default:
                    break;
            }

            // Проверка на победу
            if (playerX == finishX && playerY == finishY)
            {
                Console.SetCursorPosition(0, height);
                Console.WriteLine("Поздравляем! Вы достигли финиша!");
                break;
            }
        }
    }

    // Генерация лабиринта с помощью DFS
    private static void GenerateMaze()
    {
        maze = new char[width, height];

        // Инициализация лабиринта: всё заполняем стенами
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                maze[i, j] = '#';
            }
        }

        // Стартовая точка — всегда (1,1)
        maze[1, 1] = 'P';  // Помещаем игрока на начальную позицию

        // Стек для поиска пути
        var stack = new Stack<(int X, int Y)>();
        stack.Push((1, 1));

        // Основной алгоритм DFS для создания лабиринта
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            var neighbors = GetUnvisitedNeighbors(current.X, current.Y);

            if (neighbors.Count > 0)
            {
                stack.Push(current);

                var randomNeighbor = neighbors[random.Next(neighbors.Count)];

                // Пробиваем стену между текущей клеткой и соседом
                maze[(current.X + randomNeighbor.X) / 2, (current.Y + randomNeighbor.Y) / 2] = ' ';
                maze[randomNeighbor.X, randomNeighbor.Y] = ' ';

                stack.Push(randomNeighbor);
            }
        }

        // Определяем случайное место для финиша (в пределах лабиринта)
        do
        {
            finishX = random.Next(1, height - 1);
            finishY = random.Next(1, width - 1);
        } while (maze[finishX, finishY] != ' ');

        maze[finishX, finishY] = 'F';  // Помещаем финиш
    }

    // Функция получения непосещенных соседей
    private static List<(int X, int Y)> GetUnvisitedNeighbors(int x, int y)
    {
        var neighbors = new List<(int X, int Y)>();

        if (x > 2 && maze[x - 2, y] == '#')
        {
            neighbors.Add((x - 2, y));
        }
        if (x < width - 3 && maze[x + 2, y] == '#')
        {
            neighbors.Add((x + 2, y));
        }
        if (y > 2 && maze[x, y - 2] == '#')
        {
            neighbors.Add((x, y - 2));
        }
        if (y < height - 3 && maze[x, y + 2] == '#')
        {
            neighbors.Add((x, y + 2));
        }
        return neighbors;
    }

    // Функция для отображения лабиринта
    private static void DrawMazeAndDisplayHelp()
    {
        Console.Clear();
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                Console.Write(maze[i, j]);
            }
            Console.WriteLine();
        }

        Console.SetCursorPosition(width + 2, 0);
        Console.WriteLine("Управление:");

        Console.SetCursorPosition(width + 2, 1);
        Console.WriteLine("W - Вверх");

        Console.SetCursorPosition(width + 2, 2);
        Console.WriteLine("A - Влево");

        Console.SetCursorPosition(width + 2, 3);
        Console.WriteLine("S - Вниз");

        Console.SetCursorPosition(width + 2, 4);
        Console.WriteLine("D - Вправо");

        Console.SetCursorPosition(width + 2, 5);
        Console.WriteLine("F - Показать путь до финиша");

        Console.SetCursorPosition(width + 2, 6);
        Console.WriteLine("Esc - Завершить игру");
    }

    // Перемещение игрока
    private static void MovePlayer(int dx, int dy)
    {
        var newX = playerX + dx;
        var newY = playerY + dy;

        // Если путь виден, очищаем его
        if (pathVisible)
        {
            ClearPath();
            pathVisible = false;
        }

        // Проверка на стену: игрок не может пройти через стены ('#')
        if (maze[newX, newY] == ' ' || maze[newX, newY] == 'F')
        {
            // Очищаем старую позицию игрока
            Console.SetCursorPosition(playerY, playerX);
            Console.Write(' ');

            // Обновляем позицию игрока
            playerX = newX;
            playerY = newY;

            // Рисуем игрока на новой позиции
            Console.SetCursorPosition(playerY, playerX);
            Console.Write('P');
        }
    }

    // Поиск пути и отображение его символом '.'
    private static void ShowPathToFinish()
    {
        // Используем простой поиск пути (DFS)
        var visited = new bool[width, height];
        var path = new Stack<(int X, int Y)>();
        path.Push((playerX, playerY));
        visited[playerX, playerY] = true;

        var pathFound = DFS(playerX, playerY, visited, path);

        if (pathFound)
        {
            // Отобразим путь в виде символов '.'
            foreach (var (X, Y) in path)
            {
                if (maze[X, Y] == ' ')
                {
                    maze[X, Y] = '.';
                    Console.SetCursorPosition(Y, X);
                    Console.Write('.');
                }
            }
            pathVisible = true; // Устанавливаем, что путь виден
        }
    }

    // Рекурсивный DFS для поиска пути от игрока до финиша
    private static bool DFS(int x, int y, bool[,] visited, Stack<(int X, int Y)> path)
    {
        // Если достигли финиша
        if (x == finishX && y == finishY)
        {
            return true;
        }

        // Варианты движения (вверх, вниз, влево, вправо)
        int[] dx = [-1, 1, 0, 0];
        int[] dy = [0, 0, -1, 1];

        for (var i = 0; i < 4; i++)
        {
            var newX = x + dx[i];
            var newY = y + dy[i];

            if (newX >= 0 && newX < height && newY >= 0 && newY < width && !visited[newX, newY] && (maze[newX, newY] == ' ' || maze[newX, newY] == 'F'))
            {
                visited[newX, newY] = true;
                path.Push((newX, newY));

                if (DFS(newX, newY, visited, path))
                {
                    return true;
                }

                path.Pop();
            }
        }

        return false;
    }

    private static void ClearPath()
    {
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                if (maze[i, j] == '.')
                {
                    maze[i, j] = ' ';
                    Console.SetCursorPosition(j, i);
                    Console.Write(' ');
                }
            }
        }
    }
}
