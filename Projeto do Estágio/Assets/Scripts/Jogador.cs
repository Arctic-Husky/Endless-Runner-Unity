using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jogador : MonoBehaviour
{
    public float speed; // Velocidade do jogador
    public float laneSpeed; // Velocidade de troca de lane (posição no Vector3)
    public float jumpLength;
    public float jumpHeight;

    private Animator anim;
    private Rigidbody rb; // Variável do tipo Rigidbody
    private int currentLane = 1; // A lane atual, começando em 1
    private Vector3 verticalTargetPosition;
    private bool jumping = false;
    private float jumpStart;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Liga o código ao Rigidbody do personagem
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
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

        if (jumping)
        {
            float ratio = (transform.position.z - jumpStart) / jumpLength;
            if (ratio >= 1f)
            {
                jumping = false;
                anim.SetBool("Jumping", false);
            }
            else
            {
                verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight;
            }
        }
        else
        {
            verticalTargetPosition.y = Mathf.MoveTowards(verticalTargetPosition.y, 0, 5 * Time.deltaTime);
        }

        Vector3 targetPosition = new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);
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
        verticalTargetPosition = new Vector3((currentLane - 1), 0, 0);
    }

    void Jump()
    {
        if (!jumping)
        {
            jumpStart = transform.position.z;
            anim.SetFloat("JumpSpeed", speed / jumpLength);
            anim.SetBool("Jumping", true);
            jumping = true;
        }
    }
}
