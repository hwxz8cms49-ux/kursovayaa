using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class _Board : MonoBehaviour
{
    // размеры поля
    public int width = 9;
    public int height = 14;

    // массивы для префабов (обычных и бонусных)
    public GameObject[] Prefabs;
    public GameObject[] BonusPrefabs;

    // сетка игры
    private GameObject[,] AllAnimals;
    private GameObject CurrentAnimal;

    // расстояние между фишками
    float spacingX = 100f;
    float spacingY = 100f;
    float offsetX;
    float offsetY;

    // счет и таймер (интерфейс)
    public Text Score_;
    public TextMeshProUGUI Timer_;
    private int score = 0;
    private float timer = 60f;

    // экран окончания игры
    private bool IsGameOver = false;
    public GameObject GameOverPan;
    public TextMeshProUGUI FinalScore;
    public TextMeshProUGUI BestScore;
    void Start()
    {
        AllAnimals = new GameObject[width, height]; // массив для хранения ссылок на животных
        CreateBoard(); // создание доски с префабами
        StartCoroutine(CreateBonus()); // запуск таймера бонусов
    }
    // создание доски с префабами
    void CreateBoard()
    {
        // расчет смещения по горизонтали и вертикали, чтобы сетка была по центру экрана
        offsetX = (width - 1) * spacingX / 2f;
        offsetY = (height - 1) * spacingY / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int RandomIndex = Random.Range(0, Prefabs.Length); // случайный индекс животного из массива
                while (CoincidencesNearby(x, y, Prefabs[RandomIndex])) // проверка, чтобы не было 3 в ряд на старте
                {
                    RandomIndex = Random.Range(0, Prefabs.Length); // снова выбираем случайный индекс животного
                }

                Vector2 pos = new Vector2(x * spacingX - offsetX, y * spacingY - offsetY); // вычисление позиции, умножаем номер ячейки на шаг и вычитаем смещение, чтобы сдвинуть поле к центру

                GameObject Pet = Instantiate(Prefabs[RandomIndex], transform); // создаем на сцене выбранный префаб
                Pet.GetComponent<RectTransform>().anchoredPosition = pos; // установление префаба в нужную точку на игровом поле

                AllAnimals[x, y] = Pet; // передаем ссылку на созданный префаб в общий массив животных

                // передаем префабу его координаты в сетке
                if (Pet.GetComponent<_InteractionWithAnimals>() != null)
                {
                    _InteractionWithAnimals AnimalSettings = Pet.GetComponent<_InteractionWithAnimals>();
                    AnimalSettings.x = x; 
                    AnimalSettings.y = y;
                }
            }
        }
    }

    // проверка совпадений поблизкости при генерации
    bool CoincidencesNearby(int x, int y, GameObject Dot)
    {
        if (x > 1)
        {
            if (AllAnimals[x - 1, y] != null && AllAnimals[x - 2, y] != null)
            {
                if (AllAnimals[x - 1, y].tag == Dot.tag && AllAnimals[x - 2, y].tag == Dot.tag)
                {
                    return true;
                }
            }
        }
        if (y > 1)
        {
            if (AllAnimals[x, y - 1] != null && AllAnimals[x, y - 2] != null)
            {
                if (AllAnimals[x, y - 1].tag == Dot.tag && AllAnimals[x, y - 2].tag == Dot.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // выбор фишки игроком
    public void SelectAnimal(GameObject pet)
    {
        if (IsGameOver == true)
        {
            return;
        }
        if (CurrentAnimal == null)
        {
            CurrentAnimal = pet; // выбираем первую фишку
            _InteractionWithAnimals FirstAnimal = CurrentAnimal.GetComponent<_InteractionWithAnimals>();
            if (FirstAnimal != null)
            {
                FirstAnimal.OnOutline();
            }
        }
        else
        {
            // выбираем вторую фишку и проверяем, соседи ли они
            _InteractionWithAnimals FirstAnimal = CurrentAnimal.GetComponent<_InteractionWithAnimals>();
            if (FirstAnimal != null)
            {
                FirstAnimal.OffOutline();
            }
            int x1 = CurrentAnimal.GetComponent<_InteractionWithAnimals>().x;
            int x2 = pet.GetComponent<_InteractionWithAnimals>().x;
            int y1 = CurrentAnimal.GetComponent<_InteractionWithAnimals>().y;
            int y2 = pet.GetComponent<_InteractionWithAnimals>().y;
            int calcX = Mathf.Abs(x1 - x2);
            int calcY = Mathf.Abs(y1 - y2);
            // если соседи по горизонтал или вертикали
            if ((calcX == 1 && calcY == 0) || (calcX == 0 && calcY == 1))
            {
                StartCoroutine(SwapAnimals(CurrentAnimal, pet));
            }
            CurrentAnimal = null;
        }
    }
    // перемещение фишек местами
    IEnumerator SwapAnimals(GameObject pet1, GameObject pet2)
    {
        _InteractionWithAnimals PosPet1 = pet1.GetComponent<_InteractionWithAnimals>();
        _InteractionWithAnimals PosPet2 = pet2.GetComponent<_InteractionWithAnimals>();
        int CurrX1 = PosPet1.x;
        int CurrX2 = PosPet2.x;
        int CurrY2 = PosPet2.y;
        int CurrY1 = PosPet1.y;

        // меняем фишки местами в массиве
        GameObject box = AllAnimals[CurrX1, CurrY1];
        AllAnimals[CurrX1, CurrY1] = AllAnimals[CurrX2, CurrY2];
        AllAnimals[CurrX2, CurrY2] = box;

        // обновляем координаты внутри скриптов фишек
        PosPet1.x = CurrX2;
        PosPet2.x = CurrX1;
        PosPet1.y = CurrY2;
        PosPet2.y = CurrY1;

        // двигаем фишки на экране
        pet1.GetComponent<RectTransform>().anchoredPosition = new Vector2(CurrX2 * spacingX - offsetX, CurrY2 * spacingY - offsetY);
        pet2.GetComponent<RectTransform>().anchoredPosition = new Vector2(CurrX1 * spacingX - offsetX, CurrY1 * spacingY - offsetY);

        List<GameObject> MatchPet1 = CheckMatch(pet1);
        List<GameObject> MatchPet2 = CheckMatch(pet2);
        bool IsBonus = false;
        int bonusX = 0, bonusY = 0;

        if (MatchPet1 != null)
        {
            foreach (GameObject MatchPet in MatchPet1)
            {
                _InteractionWithAnimals Coordinate = MatchPet.GetComponent<_InteractionWithAnimals>();
                if (Coordinate != null && Coordinate.bonus)
                {
                    IsBonus = true;
                    bonusX = Coordinate.x;
                    bonusY = Coordinate.y;
                }
            }
            if (IsBonus)
            {
                DestructionByTheCross(bonusX, bonusY);
                StartCoroutine(DropAnimals());
                yield break;
            }
            foreach (GameObject MatchPet in MatchPet1)
            {
                int x1 = MatchPet.GetComponent<_InteractionWithAnimals>().x;
                int y1 = MatchPet.GetComponent<_InteractionWithAnimals>().y;
                if (AllAnimals[x1, y1] != null)
                {
                    AllAnimals[x1, y1] = null;
                    Destroy(MatchPet);
                    score += 10;
                }
            }
            Score_.text = score.ToString();
            StartCoroutine(DropAnimals());
        }
        else if (MatchPet2 != null)
        {
            foreach (GameObject MatchPet in MatchPet2)
            {
                _InteractionWithAnimals Coordinate = MatchPet.GetComponent<_InteractionWithAnimals>();
                if (Coordinate != null && Coordinate.bonus)
                {
                    IsBonus = true;
                    bonusX = Coordinate.x;
                    bonusY = Coordinate.y;
                }
            }
            if (IsBonus)
            {
                DestructionByTheCross(bonusX, bonusY);
                StartCoroutine(DropAnimals());
                yield break;
            }
            foreach (GameObject MatchPet in MatchPet2)
            {
                int x2 = MatchPet.GetComponent<_InteractionWithAnimals>().x;
                int y2 = MatchPet.GetComponent<_InteractionWithAnimals>().y;
                if (AllAnimals[x2, y2] != null)
                {
                    AllAnimals[x2, y2] = null;
                    Destroy(MatchPet);
                    score += 10;
                }
            }
            Score_.text = score.ToString();
            StartCoroutine(DropAnimals());
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            AllAnimals[CurrX1, CurrY1] = pet1;
            AllAnimals[CurrX2, CurrY2] = pet2;
            PosPet1.x = CurrX1;
            PosPet2.x = CurrX2;
            PosPet1.y = CurrY1;
            PosPet2.y = CurrY2;
            pet1.GetComponent<RectTransform>().anchoredPosition = new Vector2(CurrX1 * spacingX - offsetX, CurrY1 * spacingY - offsetY);
            pet2.GetComponent<RectTransform>().anchoredPosition = new Vector2(CurrX2 * spacingX - offsetX, CurrY2 * spacingY - offsetY);
        }
        yield return null;
    }
    // поиск три в ряд 
    List<GameObject> CheckMatch(GameObject pet)
    {
        List<GameObject> MatchList = new List<GameObject>();
        List<GameObject> MatchHorizontal = new List<GameObject>();
        List<GameObject> MatchVertical = new List<GameObject>();

        string CurrTag = pet.tag;
        int x = pet.GetComponent<_InteractionWithAnimals>().x;
        int y = pet.GetComponent<_InteractionWithAnimals>().y;

        // поиск по горизонтали
        MatchHorizontal.Add(pet);
        for (int i = x - 1; i >= 0; i--)
        {
            if (AllAnimals[i, y].tag == CurrTag)
            {
                MatchHorizontal.Add(AllAnimals[i, y]);
            }
            else
            {
                break;
            }
        }
        for (int i = x + 1; i < width; i++)
        {
            if (AllAnimals[i, y].tag == CurrTag)
            {
                MatchHorizontal.Add(AllAnimals[i, y]);
            }
            else
            {
                break;
            }
        }
        // поиск по вертикали
        MatchVertical.Add(pet);
        for (int i = y - 1; i >= 0; i--)
        {
            if (AllAnimals[x, i] != null && AllAnimals[x, i].tag == CurrTag) {
                MatchVertical.Add(AllAnimals[x, i]);
            }
            else 
            {
                break; 
            }
        }
        for (int i = y + 1; i < height; i++)
        {
            if (AllAnimals[x, i] != null && AllAnimals[x, i].tag == CurrTag)
            {
                MatchVertical.Add(AllAnimals[x, i]);
            }
            else
            {
                break;
            }
        }
        if (MatchHorizontal.Count >= 3)
        {
            MatchList.AddRange(MatchHorizontal);
        }
        if (MatchVertical.Count >= 3)
        {
            foreach (GameObject match in  MatchVertical)
            {
                if (!MatchList.Contains(match))
                {
                    MatchList.Add(match);
                }
            }
        }
        if (MatchList.Count > 0)
        {
            return MatchList;
        }
        return null;
    }

    // падение фишек вниз, если под ними пусто
    IEnumerator DropAnimals()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (AllAnimals[i, j] == null)
                {
                    for (int h = j + 1; h < height; h++)
                    {
                        if (AllAnimals[i, h] !=  null)
                        {
                            AllAnimals[i, j] = AllAnimals[i, h];
                            AllAnimals[i, h] = null;
                            _InteractionWithAnimals Move = AllAnimals[i, j].GetComponent <_InteractionWithAnimals>();
                            Move.y = j;
                            AllAnimals[i, j].GetComponent<RectTransform>().anchoredPosition = new Vector2(i * spacingX - offsetX, j * spacingY - offsetY);
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.2f);
        Refilling();
        yield return null;
    }

    // заполнение пустых мест новыми фишками
    void Refilling()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (AllAnimals[i, j] == null)
                {
                    int RandomIndex = Random.Range(0, Prefabs.Length);
                    while (CoincidencesNearby(i, j, Prefabs[RandomIndex]))
                    {
                        RandomIndex = Random.Range(0, Prefabs.Length);
                    }

                    Vector2 pos = new Vector2(i * spacingX - offsetX, j * spacingY - offsetY);

                    GameObject Tile = Instantiate(Prefabs[RandomIndex], transform);
                    Tile.GetComponent<RectTransform>().anchoredPosition = pos;

                    AllAnimals[i, j] = Tile;

                    if (Tile.GetComponent<_InteractionWithAnimals>() != null)
                    {
                        var script = Tile.GetComponent<_InteractionWithAnimals>();
                        script.x = i;
                        script.y = j;
                    }
                }
            }
        }
        ChekingTheEntieBoard();
    }

    // проверка всего поля на комбинации три в ряд после падения
    void ChekingTheEntieBoard()
    {
        for (int i =0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (AllAnimals[i, j] != null)
                {
                    List<GameObject> MatchPet = CheckMatch(AllAnimals[i, j]);
                    if (MatchPet != null)
                    {
                        foreach (GameObject MatchPet_ in MatchPet)
                        {
                            int x1 = MatchPet_.GetComponent<_InteractionWithAnimals>().x;
                            int y1 = MatchPet_.GetComponent<_InteractionWithAnimals>().y;
                            AllAnimals[x1, y1] = null;
                            Destroy(MatchPet_);
                        }
                        StartCoroutine(DropAnimals());
                        return;
                    }
                }
            }
        }
    }

    void GameOver()
    {
        IsGameOver = true;
        Timer_.text = "Время истекло.";
        Timer_.gameObject.SetActive(false);
        Score_.gameObject.SetActive(false);
        if (GameOverPan !=  null)
        {
            GameOverPan.SetActive(true);
        }
        if (FinalScore != null)
        {
            FinalScore.text = "Ваш итоговый счет: " + score.ToString();
        }
        int HighestScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > HighestScore)
        {
            HighestScore = score;
            PlayerPrefs.SetInt("HighScore", HighestScore);
            PlayerPrefs.Save();
        }
        if (BestScore  != null)
        {
            BestScore.text = "Лучший результат: " + HighestScore.ToString();
        }
    }
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            Timer_.text = Mathf.RoundToInt(timer).ToString();
        }
        else if (!IsGameOver)
        {
            GameOver();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    // создание бонусов каждые 10 секунд
    IEnumerator CreateBonus()
    {
        while (!IsGameOver)
        {
            yield return new WaitForSeconds(10f);
            if (BonusPrefabs ==  null || BonusPrefabs.Length == 0)
            {
                continue;
            }
            // список для поиска обычных животных (не бонусных)
            List<_InteractionWithAnimals> OrdinaryAnimal = new List<_InteractionWithAnimals>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (AllAnimals[x, y] != null)
                    {
                        _InteractionWithAnimals script = AllAnimals[x, y].GetComponent<_InteractionWithAnimals>();
                        if (script != null && !script.bonus)
                        {
                            OrdinaryAnimal.Add(script);
                        }
                    }
                }
            }
            // если нашли обычные фишки, превращаем одну случайную в бонус
            if (OrdinaryAnimal.Count > 0)
            {
                _InteractionWithAnimals RandomAnimal = OrdinaryAnimal[Random.Range(0, OrdinaryAnimal.Count)];
                _InteractionWithAnimals Coordinate = RandomAnimal.GetComponent<_InteractionWithAnimals>();
                ReplaceAnimalToBonus(Coordinate.x, Coordinate.y);
            }
        }
    }

    // замена обычной фишки на бонусную с передачей координат
    void ReplaceAnimalToBonus(int x, int y)
    {
        GameObject OldAnimal = AllAnimals[x, y];
        string tag = OldAnimal.tag;
        GameObject NecessaryPrefab = null;

        // ищем нужный бонусный префаб по тегу
        foreach (GameObject Prefab_ in BonusPrefabs)
        {
            if (Prefab_.CompareTag(tag))
            {
                NecessaryPrefab = Prefab_;
                break;
            }
        }
        if (NecessaryPrefab != null)
        {
            Vector2 pos = OldAnimal.GetComponent<RectTransform>().anchoredPosition;
            Destroy(OldAnimal);
            GameObject NewBonus = Instantiate(NecessaryPrefab, transform);
            NewBonus.GetComponent<RectTransform>().anchoredPosition = pos;
            AllAnimals[x, y] = NewBonus;
            _InteractionWithAnimals Coordinate = NewBonus.GetComponent<_InteractionWithAnimals>();
            if (Coordinate != null)
            {
                Coordinate.x = x;
                Coordinate.y = y;
                Coordinate.bonus = true;
            }
        }
    }

    // уничтожение вертикального и горизонтального ряда
    void DestructionByTheCross(int X, int Y)
    {
        List<GameObject> ListForDestruction = new List<GameObject>();
        // собираем строку
        for (int x = 0; x < width; x++)
        {
            if (AllAnimals[x, Y] != null)
            {
                ListForDestruction.Add(AllAnimals[x, Y]);
                AllAnimals[x, Y] = null;
            }
        }
        // собираем столбец
        for (int y = 0;  y < height; y++)
        {
            if (AllAnimals[X, y] != null)
            {
                ListForDestruction.Add(AllAnimals[X, y]);
                AllAnimals[X, y] = null;
            }
        }
        // уничтожаем и начиляем 70 очков
        foreach (GameObject animal in ListForDestruction)
        {
            Destroy(animal);
            score += 70;
        }
        Score_.text = score.ToString();
    }
}

