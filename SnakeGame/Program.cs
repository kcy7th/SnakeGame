using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        bool playAgain = true;

        while (playAgain)
        {
            // 게임 실행
            PlayGame();

            // 재시작
            Console.Write("Retry? (Y/N): ");
            char input = Console.ReadKey(true).KeyChar;
            playAgain = input == 'y' || input == 'Y';
        }
    }

    static void PlayGame()
    {
        // 게임 속도 조정
        int gameSpeed = 100;
        int foodCount = 0; // 먹은 음식 수

        // 벽
        DrawWalls();

        // 뱀의 초기 위치, 방향
        Point p = new Point(4, 5, '*');
        Snake snake = new Snake(p, 4, Direction.RIGHT);
        snake.Draw();

        // 음식 무작위 생성
        FoodCreator foodCreator = new FoodCreator(80, 20, '$');
        Point food = foodCreator.CreateFood();
        food.Draw();

        // 게임 루프
        while (true)
        {
            // 방향 변경
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        snake.direction = Direction.UP;
                        break;
                    case ConsoleKey.DownArrow:
                        snake.direction = Direction.DOWN;
                        break;
                    case ConsoleKey.LeftArrow:
                        snake.direction = Direction.LEFT;
                        break;
                    case ConsoleKey.RightArrow:
                        snake.direction = Direction.RIGHT;
                        break;
                }
            }

            // 음식 충돌 여부
            if (snake.Eat(food))
            {
                foodCount++; // 먹은 음식 수를 증가
                food.Draw();

                // 새 음식 생성
                food = foodCreator.CreateFood();
                food.Draw();
                if (gameSpeed > 10) // 게임이 점점 빠르게
                {
                    gameSpeed -= 10;
                }
            }
            else
            {
                // 음식 먹지 않은 시, 그냥 이동
                snake.Move();
            }

            Thread.Sleep(gameSpeed);

            // 벽에 부딪히거나, 몸에 부딪힐 시 게임 오버
            if (snake.IsHitTail() || snake.IsHitWall())
            {
                break;
            }

            Console.SetCursorPosition(0, 21); // 커서 위치 설정
            Console.WriteLine($"먹은 음식 수: {foodCount}"); // 먹은 음식 수 출력
        }

        WriteGameOver();  // 게임 오버 메시지 출력
    }

    static void WriteGameOver()
    {
        int xOffset = 25;
        int yOffset = 22;
        Console.SetCursorPosition(xOffset, yOffset++);
        WriteText("============================", xOffset, yOffset++);
        WriteText("         GAME OVER", xOffset, yOffset++);
        WriteText("============================", xOffset, yOffset++);
    }

    static void WriteText(string text, int xOffset, int yOffset)
    {
        Console.SetCursorPosition(xOffset, yOffset);
        Console.WriteLine(text);
    }

    // 벽 그리는 메서드
    static void DrawWalls()
    {
        // 상하 벽 그리기
        for (int i = 0; i < 80; i++)
        {
            Console.SetCursorPosition(i, 0);
            Console.Write("#");
            Console.SetCursorPosition(i, 20);
            Console.Write("#");
        }

        // 좌우 벽 그리기
        for (int i = 0; i < 20; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write("#");
            Console.SetCursorPosition(80, i);
            Console.Write("#");
        }
    }
}

public class Point
{
    public int x { get; set; }
    public int y { get; set; }
    public char sym { get; set; }

    // Point 클래스 생성자
    public Point(int _x, int _y, char _sym)
    {
        x = _x;
        y = _y;
        sym = _sym;
    }

    // 점을 그리는 메서드
    public void Draw()
    {
        Console.SetCursorPosition(x, y);
        Console.Write(sym);
    }

    // 점을 지우는 메서드
    public void Clear()
    {
        sym = ' ';
        Draw();
    }

    // 두 점이 같은지 비교하는 메서드
    public bool IsHit(Point p)
    {
        return p.x == x && p.y == y;
    }
}

// 방향 표현 열거형
public enum Direction
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}

public class Snake
{
    public List<Point> body; // 뱀의 몸통
    public Direction direction; // 뱀 현재 방향

    public Snake(Point tail, int length, Direction _direction)
    {
        direction = _direction;
        body = new List<Point>();
        for (int i = 0; i < length; i++)
        {
            Point p = new Point(tail.x, tail.y, '*');
            body.Add(p);
            tail.x += 1;  // 수평으로만 길어지게
        }
    }

    // 뱀 그리기
    public void Draw()
    {
        foreach (Point p in body)
        {
            p.Draw();
        }
    }

    // 음식과 충돌 여부
    public bool Eat(Point food)
    {
        Point head = GetNextPoint();
        if (head.IsHit(food))
        {
            // 먹은 음식 위치를 뱀의 머리에 추가
            body.Insert(0, new Point(food.x, food.y, '*')); // 새로운 머리 위치에 음식 추가
            return true;
        }
        return false;
    }

    // 뱀 이동
    public void Move()
    {
        Point tail = body.Last();
        body.Remove(tail); // 꼬리 제거
        Point head = GetNextPoint(); // 새로운 머리 계산
        body.Insert(0, head); // 새로운 머리 위치를 리스트 맨 앞에 추가

        tail.Clear(); // 이전 위치의 꼬리 지우기
        head.Draw(); // 새로운 머리 위치 그리기
    }

    // 다음 이동 위치
    public Point GetNextPoint()
    {
        Point head = body.Last();
        Point nextPoint = new Point(head.x, head.y, head.sym);
        switch (direction)
        {
            case Direction.LEFT:
                nextPoint.x -= 2; // 왼쪽으로 두 칸 이동
                break;
            case Direction.RIGHT:
                nextPoint.x += 2; // 오른쪽으로 두 칸 이동
                break;
            case Direction.UP:
                nextPoint.y -= 1; // 위로 한 칸 이동
                break;
            case Direction.DOWN:
                nextPoint.y += 1; // 아래로 한 칸 이동
                break;
        }
        return nextPoint;
    }

    // 자신의 몸과 충돌 여부
    public bool IsHitTail()
    {
        var head = body.Last();
        for (int i = 0; i < body.Count - 2; i++) // 머리와 몸통 비교
        {
            if (head.IsHit(body[i]))
                return true;
        }
        return false;
    }

    // 벽과 충돌 여부
    public bool IsHitWall()
    {
        var head = body.Last();
        if (head.x <= 0 || head.x >= 80 || head.y <= 0 || head.y >= 20)
            return true;
        return false;
    }
}

public class FoodCreator
{
    int mapWidth;
    int mapHeight;
    char sym;

    Random random = new Random();

    public FoodCreator(int mapWidth, int mapHeight, char sym)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        this.sym = sym;
    }

    // 음식 무작위 생성
    public Point CreateFood()
    {
        int x = random.Next(2, mapWidth - 2);
        // x 좌표를 2단위로 맞추기 위해 짝수로 생성
        x = x % 2 == 1 ? x : x + 1;
        int y = random.Next(2, mapHeight - 2);
        return new Point(x, y, sym);
    }
}