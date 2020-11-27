using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 Anotações: 

    1. Aparentemente não é possível alterar um único valor do Vector3 de uma vez, é preciso alterar todos ao mesmo tempo.
    2. O código não está muito otimizado, lembrar de dar uma melhorada depois

 */

public class Jogador : MonoBehaviour
{
    public float speed; // Velocidade do jogador
    public float laneSpeed; // Velocidade de troca de lane (posição no Vector3)
    public float jumpLength; // Distância máxima do pulo
    public float jumpHeight; // Altura máxima do pulo
    public float slideLength; // Distância máxima do pulo
    public int maxlife;
    public float minSpeed = 10f;
    public float maxSpeed = 30f;
    public float invincibleTime;
    public GameObject model;

    private Animator anim;
    private Rigidbody rb; // Variável do tipo Rigidbody
    private BoxCollider boxCollider;
    private int currentLane = 1; // A lane atual, começando em 1
    private Vector3 verticalTargetPosition;
    private bool jumping = false;
    private float jumpStart;
    private bool sliding = false;
    private float slideStart;
    private Vector3 boxColliderSize; // Guarda o tamanho inicial do boxCollider
    private int currentLife;
    private bool invincible = false;
    private UIManager uiManager;
    private int coins;
    private int spendCoins;
    private float score;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Liga o código ao Rigidbody do personagem
        anim = GetComponentInChildren<Animator>(); // **
        boxCollider = GetComponent<BoxCollider>(); // **
        boxColliderSize = boxCollider.size; // Pega o tamanho inicial do boxCollider
        currentLife = 1;
        speed = minSpeed;
        uiManager = FindObjectOfType<UIManager>();
        uiManager.UpdateLives(currentLife);
    }

    // Update is called once per frame
    void Update()
    {
        score += Time.deltaTime * speed;
        uiManager.UpdateScore((int)score);

        if (Input.GetKeyDown(KeyCode.LeftArrow)) // Se pressionar a seta da esquerda
        {
            ChangeLane(-1); // Chamada da função ChangeLane(-1);, onde o -1 é o parâmetro que representa a direção aonde o jogador deseja se movimentar, que neste caso é para a esquerda
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) // Se pressionar a seta da direita
        {
            ChangeLane(1); // **
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            Slide();
        }

        if (jumping)
        {
            float ratio = (transform.position.z - jumpStart) / jumpLength; // (Posição atual no eixo z) menos (posição em z quando o jogador pulou) dividido por (tamanho máximo do pulo)
            if (ratio >= 1f) // Se ratio >= 1, fim do pulo
            {
                jumping = false;
                anim.SetBool("Jumping", false);
            }                                                               // Essa parte do código é responsável para garantir que com o aumento da velocidade o jogador esteja no ar por mais tempo
            else
            {
                verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight; // Fórmula que controla a altura em relação ao chão do jogador durante o pulo
                                                                                     // Ela divide o valor em ratio, que é sempre menor que 1, e multiplica por PI. É feito o cálculo
                                                                                     // do seno com o resultado, e o resultado do seno é multiplicado pela altura máxima do pulo.
                                                                                     // O resultado disso tudo é então armazenado em verticalTargetPosition.y e a posição do jogador é
                                                                                     // atualizada a cada frame nas linhas abaixo
            }
        }
        else
        {
            verticalTargetPosition.y = Mathf.MoveTowards(verticalTargetPosition.y, 0, 5 * Time.deltaTime); // Leva o jogador ao chão se não estiver em pulo, simulando gravidade
        }

        if (sliding)
        {
            float ratio = (transform.position.z - slideStart) / slideLength; // Controla o quão longo o slide vai ser, funciona que nem o pulo, tendo a mesma longevidade independente da velocidade
            if(ratio >= 1f)
            {
                sliding = false;
                anim.SetBool("Sliding", false);
                boxCollider.size = boxColliderSize;
            }
        }

        Vector3 targetPosition = new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z); // Transform -> manipula a posição, rotação e escala de um objeto. Todo transform pode participar de uma hierarquia.
                                                                                                                        // Esta linha de código é responsável por pegar a próxima posição do jogador na cena, aonde o jogador quer ir
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime); // Atualiza a posição do jogador para a desejada por meio da transform.position e a MoveTowards, passando para a mesma a nossa posição
                                                                                                                  // atual, nossa posição desejada, e a velocidade para mudar de lane, e utilizando o Time.deltaTime para essa troca não ser dependente dos frames
        if(spendCoins == 100)
        {
            spendCoins = 0;
            if (currentLife == maxlife)
                return;
            else
            {
                currentLife++;
                uiManager.UpdateLives(currentLife);
            }
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector3.forward * speed;
    }

    void ChangeLane(int direction)
    {
        int targetLane = currentLane + direction; // Lane aonde o jogador pretende se deslocar
        if (targetLane < 0 || targetLane > 2) // Se a Lane alvo for menor que 0 ou maior que 2, o procedimento acaba por meio do return
            return;
        currentLane = targetLane; // Depois de testar a condição da posição desejada, a Lane atual recebe a Lane alvo
        verticalTargetPosition = new Vector3((currentLane - 1), 0, 0); // Atualiza o vetor
    }

    void Jump()
    {
        if (!jumping)
        {
            jumpStart = transform.position.z; // Guarda a posição em z de quando a função Jump() foi chamada
            anim.SetFloat("JumpSpeed", speed / jumpLength); // Altera a rapidez da animação de pulo de acordo com a velocidade dividido pelo tamanho máximo do pulo
            anim.SetBool("Jumping", true);
            jumping = true;
        }
    }

    void Slide()
    {
        if(!sliding && !jumping)
        {
            slideStart = transform.position.z; // Posição do personagem ao começatr o slide
            anim.SetFloat("JumpSpeed", speed / slideLength);
            anim.SetBool("Sliding", true);
            Vector3 newSize = boxCollider.size; // Variável  do tipo Vector3 que recebe o tamanho do boxCollider
            newSize.y = newSize.y / 2; // Tamanho do boxCollider é diminuido à metade
            boxCollider.size = newSize; // Tamanho do boxCollider é atualizado com o novo valor dentro de newSize
            sliding = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Coin"))
        {
            coins++;
            spendCoins++;
            uiManager.UpdateCoins(coins);
            other.transform.parent.gameObject.SetActive(false);
        }
        if (invincible)
            return;
        if(other.CompareTag("Obstacle"))
        {
            currentLife--;
            uiManager.UpdateLives(currentLife);
            anim.SetTrigger("Hit");
            speed = 0;
            if(currentLife <= 0)
            {
                speed = 0;
                anim.SetBool("Dead", true);
                uiManager.gameOverPanel.SetActive(true);
                Invoke("CallMenu", 2f);
            }
            else
            {
                StartCoroutine(Blinking(invincibleTime));
            }
        }
    }

    IEnumerator Blinking(float time)
    {
        invincible = true;
        float timer = 0;
        float currentBlink = 1f;
        float lastBlink = 0;
        float blinkPeriod = 0.1f;
        bool enabled = false;
        yield return new WaitForSeconds(1f);
        speed = minSpeed;
        while(timer < time && invincible)
        {
            model.SetActive(enabled);
            yield return null;
            timer += Time.deltaTime;
            lastBlink += Time.deltaTime;
            if(blinkPeriod < lastBlink)
            {
                lastBlink = 0;
                currentBlink = 1f - currentBlink;
                enabled = !enabled;
            }
        }
        model.SetActive(true);
        invincible = false;
    }

    void CallMenu()
    {
        GameManager.gm.EndRun();
    }

    public void IncreaseSpeed()
    {
        speed *= 1.15f;
        if (speed >= maxSpeed)
            speed = maxSpeed;
    }
}
