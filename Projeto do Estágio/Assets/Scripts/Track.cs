using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public GameObject[] obstacles;
    public Vector2 numberOfObstacles;
    public GameObject coin;
    public Vector2 numberOfCoins;

    public List<GameObject> newObstacles;
    public List<GameObject> newCoins;

    // Start is called before the first frame update
    void Start()
    {
        int newNumberOfObstacles = (int)Random.Range(numberOfObstacles.x, numberOfObstacles.y); // Vetor com 2 posiçõees que armazena um valor aleatório entre os escolhidos através do unity
        int newNumberOfCoins = (int)Random.Range(numberOfCoins.x, numberOfCoins.y);

        for (int i = 0; i < newNumberOfObstacles; i++)
        {
            newObstacles.Add(Instantiate(obstacles[Random.Range(0, obstacles.Length)], transform)); // Instancia obstáculos da lista de obstáculos
            newObstacles[i].SetActive(false); // Deixar os obstáculos inativos por enquanto
        }

        for (int i = 0; i < newNumberOfCoins; i++)
        {
            newCoins.Add(Instantiate(coin, transform));
            newCoins[i].SetActive(false);
        }

        PositionateObstacles();
        PositionateCoins();
    }

    void PositionateObstacles()
    {
        for (int i = 0; i < newObstacles.Count; i++) // Tamanho da pista 114.3
        {
            float posZMin = (114.3f / newObstacles.Count) + (114.3f / newObstacles.Count) * i; // Posição mínima em Z que o obstáculo será posicionado
            float posZMax = (114.3f / newObstacles.Count) + (114.3f / newObstacles.Count) * i + 1; // Posição máxima em Z que o obstáculo será posicionado
            newObstacles[i].transform.localPosition = new Vector3(0, 0, Random.Range(posZMin, posZMax)); // Posiciona o obstáculo da List newObstacles 
            newObstacles[i].SetActive(true); // Ativa os obstáculos
            if (newObstacles[i].GetComponent<ChangeLane>() != null)
                newObstacles[i].GetComponent<ChangeLane>().PositionLane();
        }
    }
    void PositionateCoins()
    {
        float minZPos = 10f;
        for (int i = 0; i < newCoins.Count; i++)
        {
            float maxZPos = minZPos + 5f;
            float randomZPos = Random.Range(minZPos, maxZPos);
            
            newCoins[i].transform.localPosition = new Vector3(transform.position.x, transform.position.y, randomZPos);
            newCoins[i].SetActive(true);
            newCoins[i].GetComponent<ChangeLane>().PositionLane();
            minZPos = randomZPos + 1; // Evitar que uma moeda nasça em cima de outra
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<Jogador>().IncreaseSpeed();
            transform.position = new Vector3(0, 0, transform.position.z + 114.3f * 2); // Ao passar pelo trigger, a posição da pista anterior é colocada no final da atual
            PositionateObstacles();
            PositionateCoins();
        }
    } 
}
